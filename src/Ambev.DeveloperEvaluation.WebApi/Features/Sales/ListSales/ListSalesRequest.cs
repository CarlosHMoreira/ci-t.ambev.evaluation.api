namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

public class ListSalesRequest : Dictionary<string,string>
{
    public int? Page => GetIntOrNull("_page");
    
    public int? Size => GetIntOrNull("_size");
    
    public string? Order => TryGetValue("_order", out var value) ? value : null;
    private int? GetIntOrNull(string key)
    {
        if (TryGetValue(key, out var value) && int.TryParse(value, out var intValue))
        {
            return intValue;
        }
        return null;
    }
}

