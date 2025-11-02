# Setup local no Mac ou Linux
Repare que o setup local no Windows difere um pouco do Mac/Linux devido como os certificados HTTPS são gerenciados e os caminhos.

## Caminho do diretório do usuário
Primeiramente, configure no `.env` qual o valor a variável deve considerar
para o `HOME`, isso faz com que os volumes que mapeiam secrets de desenvolvimento (User secrets) e certificados de desenvolvimento.
Em ambientes Linux ou MacOS o caminho do diretório home do usuário é obtido pela variável de ambiente `HOME`, então defina no `.env`
```shell
HOME=$HOME
```   
Já em ambientes Windows, geralmente a variável `APPDATA` é utilizada para apontar para o diretório do usuário, então no `.env` ficaria:
```shell
HOME=${APPDATA}
```

## Certificado HTTPS de desenvolvimento
Ao subir o docker-compose irá notar que o serviço `ambev.developerevaluation.webapi` para sem muita informação.
Um dos cenários identificados é que o certificado https local não foi encontrado, para resolver isso siga os passos abaixo.

Gere o PFX na máquina local:
```shell
dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx
```

Confie o certificado localmente:
```shell
dotnet dev-certs https --trust
```

O certificado gerado será utilizado, pois o volume mapeia a pasta `${HOME}/.aspnet/https` para dentro do container.

Adicione ao arquivo `docker-compose.yaml` em environment do serviço `ambev.developerevaluation.webapi` as variáveis abaixo:
```yaml
    - ASPNETCORE_Kestrel__Certificates__Default__Path=/home/app/.aspnet/https/aspnetapp.pfx
    - ASPNETCORE_Kestrel__Certificates__Default__Password=Passw0rd!
```