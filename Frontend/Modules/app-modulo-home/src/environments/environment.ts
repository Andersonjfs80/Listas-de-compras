export const environment = {
    production: false,
    name: 'Development',
    appName: 'APP-MODULO-HOME',
    enableConsole: true,
    apiUrls: {
        autenticacao: 'http://localhost:5006/app-api-autenticacao',
        cadastro: 'http://localhost:5022/app-api-cadastro',
        produto: 'http://localhost:5022/app-api-cadastro',
        listaCompras: 'http://localhost:5022/app-api-cadastro',
        logs: 'http://localhost:5006/app-api-autenticacao/logs'
    },
    secretKey: 'EssaEhUmaChaveMestraSuperSecretaParaOProjetoListasDeCompras2026!',
    enableBodyEncryption: true
};

