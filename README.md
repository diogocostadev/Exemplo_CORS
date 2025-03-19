# CORS em ASP.NET Core

Este projeto demonstra a implementação de Cross-Origin Resource Sharing (CORS) em uma API ASP.NET Core. CORS é um mecanismo de segurança implementado pelos navegadores que controla requisições HTTP entre diferentes origens (domínios, protocolos ou portas).

## Características

- Implementação de diferentes políticas CORS
- Demonstração de permissões por origem, método e cabeçalho
- Controle granular de CORS por rota
- Suporte a credenciais e cookies entre origens
- Demonstração de requisições preflight OPTIONS
- Configuração de CORS para ambientes de desenvolvimento e produção

## Requisitos

- .NET 6.0 ou superior
- Visual Studio 2022 ou VS Code

## Como executar

1. Clone o repositório:
   ```
   git clone https://github.com/seu-usuario/CORSDemo.git
   cd CORSDemo
   ```

2. Restaure os pacotes:
   ```
   dotnet restore
   ```

3. Execute o projeto:
   ```
   dotnet run
   ```

4. A API estará disponível em:
   - https://localhost:7xxx (HTTPS)
   - http://localhost:5xxx (HTTP)

## Estrutura do Projeto

- **Controllers/CorsController.cs**: Endpoints para demonstração de diferentes configurações CORS
- **Program.cs**: Configuração das políticas CORS e middleware

## Políticas CORS Implementadas

Este projeto demonstra três políticas CORS principais:

1. **AllowAll** (para testes apenas, não use em produção)
   - Permite qualquer origem, método e cabeçalho
   - Usada para demonstrar uma configuração permissiva

2. **ProductionPolicy**
   - Lista explícita de origens permitidas (`https://app.seudominio.com`, `https://admin.seudominio.com`)
   - Lista explícita de métodos permitidos (GET, POST, PUT, DELETE)
   - Lista explícita de cabeçalhos permitidos (Authorization, Content-Type)
   - Permite credenciais (cookies, auth headers)

3. **DevPolicy**
   - Origens específicas para ambiente de desenvolvimento (`http://localhost:3000`, `http://localhost:8080`)
   - Permite qualquer método e cabeçalho
   - Permite credenciais

## Endpoints da API

- **GET /api/cors/default**: Usa a política CORS global
- **GET /api/cors/specific**: Usa a política AllowAll específica (sobrescreve a global)
- **GET /api/cors/disabled**: Desativa CORS explicitamente para esta rota
- **POST /api/cors/methodtest**: Testa CORS com método POST
- **OPTIONS /api/cors/preflight**: Demonstra requisição preflight
- **GET /api/cors/credentials**: Testa CORS com cookies e credenciais

## Testando CORS

### Usando o Arquivo HTTP

O arquivo `.http` incluído permite testar as diferentes configurações CORS:

1. Abra o arquivo `requests.http`
2. Execute as requisições individualmente
3. Observe os headers nas respostas, especialmente `Access-Control-Allow-*`

Importante: O cliente REST do VS Code/Visual Studio não simula completamente o comportamento de CORS de um navegador. As requisições serão enviadas, mas o cliente não bloqueará respostas como um navegador faria.

### Usando Aplicação Web de Teste

Para testar CORS de forma realista:

1. Inicie sua API ASP.NET Core
2. Crie uma página HTML simples:

```html
<!DOCTYPE html>
<html>
<head>
    <title>Teste de CORS</title>
</head>
<body>
    <h1>Teste de CORS</h1>
    <button onclick="testDefault()">Testar Rota Default</button>
    <button onclick="testSpecific()">Testar Rota Específica</button>
    <button onclick="testDisabled()">Testar Rota Desativada</button>
    <button onclick="testPost()">Testar POST</button>
    <button onclick="testCredentials()">Testar com Credenciais</button>
    
    <div id="result" style="margin-top: 20px; padding: 10px; border: 1px solid #ccc;"></div>
    
    <script>
        const apiUrl = "http://localhost:5174";
        const resultDiv = document.getElementById("result");
        
        async function fetchAndDisplay(url, options = {}) {
            resultDiv.innerHTML = "Aguarde...";
            try {
                const response = await fetch(url, options);
                const data = await response.json();
                resultDiv.innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
            } catch (error) {
                resultDiv.innerHTML = `<p style="color: red">Erro: ${error.message}</p>`;
                console.error(error);
            }
        }
        
        function testDefault() {
            fetchAndDisplay(`${apiUrl}/api/cors/default`);
        }
        
        function testSpecific() {
            fetchAndDisplay(`${apiUrl}/api/cors/specific`);
        }
        
        function testDisabled() {
            fetchAndDisplay(`${apiUrl}/api/cors/disabled`);
        }
        
        function testPost() {
            fetchAndDisplay(`${apiUrl}/api/cors/methodtest`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    name: "Teste de CORS",
                    message: "Esta é uma requisição POST"
                })
            });
        }
        
        function testCredentials() {
            fetchAndDisplay(`${apiUrl}/api/cors/credentials`, {
                credentials: "include"
            });
        }
    </script>
</body>
</html>
```

3. Salve como `cors-test.html`
4. Sirva esta página em um servidor local diferente:
   - Usando Node.js: `npx serve`
   - Usando Python: `python -m http.server 3000`
   - Ou abra diretamente no navegador (file://...)

## Como testar no desenvolvimento

Para testar CORS durante o desenvolvimento:

1. **Configure o ambiente para desenvolvimento**:
   - Em `Program.cs`, certifique-se que `app.Environment.IsDevelopment()` está detectando corretamente o ambiente
   - No ambiente de desenvolvimento, use `app.UseCors("DevPolicy")`

2. **Inicie dois servidores locais**:
   - Sua API ASP.NET Core (ex: localhost:5174)
   - Um servidor front-end para a página HTML (ex: localhost:3000)

3. **Observe o Console do Navegador**:
   - Abra as ferramentas de desenvolvedor (F12)
   - A aba Console mostrará erros de CORS caso ocorram
   - A aba Network permitirá ver os cabeçalhos CORS nas requisições/respostas

4. **Cenários comuns para testar**:
   - **Requisição simples**: GET sem cabeçalhos personalizados
   - **Requisição com preflight**: POST com cabeçalhos personalizados
   - **Requisição com credenciais**: Enviar/receber cookies

5. **Debugar problemas**:
   - Verifique os logs da aplicação ASP.NET Core
   - Confirme que a política CORS correta está sendo aplicada
   - Verifique se os cabeçalhos `Access-Control-Allow-*` estão presentes na resposta

## Considerações para Produção

- Nunca use `AllowAnyOrigin()` em conjunto com `AllowCredentials()` (não é permitido pela especificação)
- Liste apenas as origens que realmente precisam acessar sua API
- Seja específico com os métodos e cabeçalhos permitidos
- Configure CORS corretamente para ambientes de nuvem/containers
- Considere usar variáveis de ambiente para configurar origens permitidas

## Exemplos de erros comuns de CORS

- **"Origin não permitida"**: A origem da requisição não está na lista de origens permitidas
- **"Método não permitido"**: O método HTTP não está na lista de métodos permitidos
- **"Cabeçalho não permitido"**: Um cabeçalho personalizado não está na lista de cabeçalhos permitidos
- **"Credenciais não permitidas"**: A política não permite credenciais, mas o cliente está tentando enviá-las

## Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo LICENSE para detalhes.