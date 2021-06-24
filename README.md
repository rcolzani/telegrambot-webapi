<h1 align="center">
     ⏰ <a href="#" alt="media hotkeys"> TelegramBot - Reminder </a>
</h1>

<h3 align="center">
    Crie lembretes pessoais ou seja avisado do nível do rio em Blumenau/SC através do Telegram. 😁  
</h3>

<h4 align="center">
	🚧   Em desenvolvimento  🚧
</h4>

# Tabela de conteúdos

<!--ts-->

- [Sobre o projeto](#-sobre-o-projeto)
- [Funcionalidades](#-funcionalidades)
- [Como executar o projeto](#-como-executar-o-projeto)
- [Tecnologias](#-tecnologias)
- [Autor](#-autor)
<!--te-->

## 💻 Sobre o projeto

Este projeto desenvolvido através de um bot no Telegram tem duas funcionalidades principais:

#### 1 - 💦 Receber a medição do nível do rio de Blumenau/SC.

A cidade de Blumenau sofre com enchentes e enxurradas ocasionalmente. Como alguns pontos da cidade podem ficar alagados ou ilhados, nesses momentos é comum as pessoas acessarem o site do <a href="http://alertablu.cob.sc.gov.br/d/nivel-do-rio">AlertaBlu</a> para estarem atualizadas da medição do nível do rio.

Com este projeto os usuários podem receber alertas atualizados com o nível atual do rio através do Telegram. Os dados são obtidos diretamente no site do <a href="http://alertablu.cob.sc.gov.br/d/nivel-do-rio"> AlertaBlu/Defesa Civil</a> e enviados a partir de um bot no Telegram.

<sub>Cadastro para receber a medição do nível do rio</sub>  
![Cadastro de lembretes no Telegram](/screenshots/TelegramNivelDoRio.jpeg?raw=true "Cadastro para receber a medição do nível do rio")

#### 2 - ⏰ Criar lembrete

Crie um lembrete e seja avisado através do telegram todos os dias no horário cadastrado. Você pode, por exemplo, criar um lembrete para beber água, se medicar, tirar o lixo, se exercitar...

<sub>Exemplo de cadastro no Telegram</sub>  
![Cadastro de lembretes no Telegram](/screenshots/TelegramCadastroLembrete.jpeg?raw=true "Cadastro de lembretes no Telegram")

##### Outros screenshots das telas

<sub>Teclado com botões personalizados</sub>  
![Teclado com botões configuráveis](/screenshots/TelegramReplyKeyboard.jpeg?raw=true "Teclado com botões configuráveis")

<sub>Frontend em ReactJS para monitoramento das mensagens. É atualizado a partir do servidor através do SignalR.</sub>  
![Frontend em ReactJS](/screenshots/FrontendMonitoramentoMensagens.png?raw=true "Frontend em ReactJS")

##### Como utilizar

Comece enviando '/start' ou 'Olá' e o bot responderá com os comandos disponíveis. Para cadastrar um novo lembrete é bem fácil:

1. Crie um token e execute a aplicação, conforme descrito em [Como executar o projeto](#-como-executar-o-projeto)
2. Envie 'Olá' para o bot criado
3. O bot responderá com algumas instruções e mudará o teclado do celular para as opções disponíveis
4. Selecione a opção desejada

---

## ⚙️ Funcionalidades

- [x] Criar um lembrete por usuário
- [x] Consultar lembretes criados
- [x] Teclado de botões personalizados para seleção de opções
- [x] Captar os comandos e executar as ações
- [x] Armazenar em banco de dados
- [x] Monitorar o site do AlertaBlu e identificar mudanças no nível do rio
- [x] Ter uma página para visualizar as mensagens enviadas/recebidas
- [x] Enviar as mensagens enviadas/recebidas para o frontend ativamente com SinglaR
- [x] Criar vários lembretes para um usuário
- [ ] Conseguir excluir apenas um lembrete. Atualmente são excluídos todos lembretes

---

## 🚀 Como executar o projeto

1. Renomear o arquivo \_appsettings.json para appsettings.json
2. Gerar um token através do @BotFather do Telegram (Envie /start para o usuário @BotFather no Telegram)
3. No appsettings.json, alterar o conteúdo "YourAPIKey" da chave "TelegramBotToken" com o token obtido através do @BotFather
4. Fazer o build da aplicação

---

## 🛠 Tecnologias e libs

##### Backend

- .NET Core 3.1
- MySQL
- Entity Framework
- <a href="https://core.telegram.org/bots/api">Telegram Bot API</a>
- <a href="https://github.com/TelegramBots/Telegram.Bot">.NET Client for Telegram Bot API</a>
- SignalR
- <a href="https://sentry.io">Sentry</a>

##### Frontend

- <a href="https://reactjs.org"> ReactJS </a>
- <a href="https://styled-components.com/">Styled Components </a>

##### Publicação

-  <a href="https://www.netlify.com/">Netlify</a> - para publicação do frontend em react.
-  <a href="https://heroku.com/">Heroku</a> - para publicação do backend. Foi utilizado geração com docker-compose e hospedado no heroku.
-  <a href="https://www.jawsdb.com/">JawsDB</a> - para banco de dados as a service. 

---

## 🦸 Autor

<a href="https://www.linkedin.com/in/ricardocolzani/">
 <img style="border-radius: 50%;" src="https://avatars1.githubusercontent.com/u/6742811?s=400&u=08e0915ca288e05e885b4bde2193c5cc23d763c9&v=4" width="100px;" alt=""/>
 <br />
 <sub><b>Ricardo Colzani</b></sub></a> <a href="https://www.linkedin.com/in/ricardocolzani/" title="Ricardo Colzani Linkedin"></a>
 <br />

[![Linkedin Badge](https://img.shields.io/badge/-Ricardo-blue?style=flat-square&logo=Linkedin&logoColor=white&link=https://www.linkedin.com/in/ricardocolzani/)](https://www.linkedin.com/in/ricardocolzani/)

---

Feito com ❤️ por Ricardo Colzani 👋🏽 [Entre em contato!](https://www.linkedin.com/in/ricardocolzani/)
