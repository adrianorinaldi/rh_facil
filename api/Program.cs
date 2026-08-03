using Microsoft.EntityFrameworkCore;
using RhFacil.Api.Data;

// =====================================================================
// 1. CONFIGURAÇÃO INICIAL (BUILDER)
// =====================================================================

// Cria um "construtor" (builder) para a aplicação web. Ele é responsável
// por carregar configurações (como o appsettings.json) e registrar serviços.
var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// 2. REGISTRO DE SERVIÇOS (INJEÇÃO DE DEPENDÊNCIA)
// =====================================================================

// Registra os serviços necessários para gerar a documentação da API (Swagger).
// Isso permite que o Swagger descubra todas as rotas (endpoints) que criamos.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Busca a string de conexão (dados de acesso ao banco) no arquivo appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registra o Entity Framework Core no contêiner de Injeção de Dependência.
// Configura o AppDbContext (nossa classe de banco de dados) para usar o PostgreSQL,
// passando a string de conexão que acabamos de pegar acima.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configura o CORS (Cross-Origin Resource Sharing).
// O CORS é um mecanismo de segurança dos navegadores. Como nosso frontend (Vue)
// roda em uma porta (5173) e a API em outra (5031), precisamos liberar a comunicação.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", // Nome da política de segurança que estamos criando
        policy =>
        {
            policy.AllowAnyOrigin()   // Permite requisições de qualquer site/IP (localhost:5173)
                  .AllowAnyMethod()   // Permite qualquer método HTTP (GET, POST, PUT, DELETE)
                  .AllowAnyHeader();  // Permite qualquer cabeçalho na requisição
        });
});

// Registra os "Controllers" da nossa aplicação.
// Isso diz para a API procurar as classes que herdam de ControllerBase (como o EmployeesController)
// e mapear as funções delas para rotas HTTP (URLs).
builder.Services.AddControllers();

// =====================================================================
// 3. CONSTRUÇÃO DA APLICAÇÃO (APP)
// =====================================================================

// Após registrarmos todos os serviços, mandamos o construtor compilar e "construir"
// o aplicativo. A partir dessa linha, não registramos mais serviços, apenas
// configuramos como a aplicação vai responder às requisições (Pipeline).
var app = builder.Build();

// =====================================================================
// 4. CONFIGURAÇÃO DO PIPELINE DE REQUISIÇÕES (MIDDLEWARES)
// =====================================================================

// Verifica se estamos rodando em ambiente de Desenvolvimento.
if (app.Environment.IsDevelopment())
{
    // Habilita a geração do arquivo JSON do Swagger
    app.UseSwagger();
    
    // Habilita a interface gráfica do Swagger (aquela página bonita pra testar a API)
    app.UseSwaggerUI();
}

// Redireciona requisições HTTP normais para HTTPS (conexão segura).
app.UseHttpsRedirection();

// Aplica a política de CORS que criamos lá em cima ("AllowAll").
// Isso DEVE ser colocado antes de mapear os controllers, para que a segurança
// libere o tráfego antes de tentar acessar os dados.
app.UseCors("AllowAll");

// Mapeia todas as rotas (URLs) com base nas anotações [Route] dos Controllers.
// É isso que faz o "/api/employees" funcionar.
app.MapControllers();

// =====================================================================
// 5. INICIALIZAÇÃO E MIGRAÇÃO DO BANCO DE DADOS
// =====================================================================

// Cria um "escopo" temporário para acessar os serviços do banco de dados antes da API iniciar.
using (var scope = app.Services.CreateScope())
{
    // Solicita uma instância do nosso AppDbContext para o injetor de dependências.
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // O EnsureCreated() garante que o banco de dados (e as tabelas) sejam criados
    // fisicamente no PostgreSQL caso ainda não existam. Ideal para ambientes de demonstração/MVP.
    db.Database.EnsureCreated(); 
}

// =====================================================================
// 6. INICIAR SERVIDOR
// =====================================================================

// Liga de fato o servidor web (Kestrel) e fica escutando na porta configurada (ex: 5031)
// aguardando as requisições chegarem.
app.Run();
