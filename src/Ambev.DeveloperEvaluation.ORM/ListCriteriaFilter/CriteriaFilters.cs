using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Ambev.DeveloperEvaluation.ORM.ListCriteriaFilter;

public enum SearchPatternMatch
{
    Exact,
    StartsWith,
    EndsWith,
    Contains,
}

public sealed class CriteriaFilters : Dictionary<string, string>
{
    private readonly Dictionary<string, Func<IQueryable, string?, IQueryable>> _filterHandlers = new();
    private static readonly Regex PropertyNamePattern = new(@"^[A-Z][A-Za-z0-9]*$", RegexOptions.Compiled);
    
    private const string MinPrefix = "_min";
    private const string MaxPrefix = "_max";
    private CriteriaFilters(IDictionary<string, string> initial) : base(initial) { }
    public static CriteriaFilters FromDictionary(IDictionary<string, string> dictionary) => new(dictionary);
    
    public CriteriaFilters WhenMinNumeric<TEntity>(
        Expression<Func<TEntity, decimal>> forProperty, 
        Func<IQueryable<TEntity>, decimal, IQueryable<TEntity>> applyFilter
    ) => WhenNumericDecimal(MinPrefix, forProperty, applyFilter);
    
    public CriteriaFilters WhenMaxNumeric<TEntity>(
        Expression<Func<TEntity, decimal>> forProperty, 
        Func<IQueryable<TEntity>, decimal, IQueryable<TEntity>> applyFilter
    ) => WhenNumericDecimal(MaxPrefix, forProperty, applyFilter);

    private CriteriaFilters WhenNumericDecimal<TEntity>(
        string prefix,
        Expression<Func<TEntity, decimal>> forProperty, 
        Func<IQueryable<TEntity>, decimal, IQueryable<TEntity>> applyFilter
    )
    {
        ArgumentNullException.ThrowIfNull(forProperty);
        ArgumentNullException.ThrowIfNull(applyFilter);
        var memberName = ExtractMemberName(forProperty.Body);
        var key = BuildConventionKey(prefix, memberName);
        
        var prepareValue = new Func<IQueryable<TEntity>, string?, IQueryable<TEntity>>((query, value) =>
        {
            if (value == null || !decimal.TryParse(value.ToString(), out var maxValue))
            {
                return query;
            }

            return applyFilter(query, maxValue);
        });
        
        
        _filterHandlers.Add(key, (query, value) => prepareValue((IQueryable<TEntity>)query, value));

        return this;
    }

    public CriteriaFilters WhenSearchingTerm<TEntity>(
        Expression<Func<TEntity, string>> forProperty, 
        Func<IQueryable<TEntity>, string, SearchPatternMatch, IQueryable<TEntity>> applyFilter
    )
    {
        ArgumentNullException.ThrowIfNull(forProperty);
        ArgumentNullException.ThrowIfNull(applyFilter);
        var memberName = ExtractMemberName(forProperty.Body);
        var key = memberName.ToLower();
        
        var prepareValue = new Func<IQueryable<TEntity>, string?, IQueryable<TEntity>>((query, value) =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return query;
            }
            

            var searchTerm = value.Replace("*", string.Empty).Trim();
            var patternMatch = value switch
            {
                _ when value.StartsWith('*') && value.EndsWith('*') => SearchPatternMatch.Contains,
                _ when value.StartsWith('*') => SearchPatternMatch.EndsWith,
                _ when value.EndsWith('*') => SearchPatternMatch.StartsWith,
                _ => SearchPatternMatch.Exact,
            };
            
            return applyFilter(query, searchTerm, patternMatch);
        });
        
        
        _filterHandlers.Add(key, (query, value) => prepareValue((IQueryable<TEntity>)query, value));

        return this;
        
    }
    
    public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> source) => 
        _filterHandlers.Aggregate(source, (current, filter) =>
        {
            var (filterKey, handler) = filter;
            if (TryGetValue(filterKey, out var receivedValue))
            {
                return (IQueryable<TEntity>)handler(current, receivedValue);
            }

            
            return current;
        });

    private static string ExtractMemberName(Expression body)
    {
        return body switch
        {
            MemberExpression me => me.Member.Name,
            UnaryExpression { Operand: MemberExpression me } => me.Member.Name,
            _ => throw new ArgumentException("Selector deve apontar para uma propriedade simples.")
        };
    }
    
    private static string BuildConventionKey(string prefix, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("Nome de propriedade vazio.");
        }
        
        if (!PropertyNamePattern.IsMatch(propertyName))
        {
            throw new ArgumentException($"Nome de propriedade inválido, deve seguir em pascal case: {propertyName}");
        }
        
        return prefix + propertyName;
    }
}