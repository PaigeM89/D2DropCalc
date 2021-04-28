// Note this only includes basic configuration for development mode.
// For a more comprehensive configuration check:
// https://github.com/fable-compiler/webpack-config-template

var path = require("path");

// If we're running webpack-dev-server, assume we're in development
var isProduction = !hasArg(/webpack-dev-server/);
var outputWebpackStatsAsJson = hasArg('--json');

if (!outputWebpackStatsAsJson) {
    console.log("Bundling CLIENT for " + (isProduction ? "production" : "development") + "...");
}

module.exports = {
    mode: isProduction ? 'production' : 'development',
    entry: "./src/App.fsproj", 
    output: {
        path: path.join(__dirname, "../D2DropCalc.Server/WebRoot/scripts/"),
        filename: "bundle.js",
    },
    devServer: {
        publicPath: "/",
        contentBase: "./public",
        port: 8080,
    },
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: "fable-loader"
        }],
    }
}

function resolve(filePath) {
    return path.isAbsolute(filePath) ? filePath : path.join(__dirname, filePath);
}

function hasArg(arg) {
    return arg instanceof RegExp
        ? process.argv.some(x => arg.test(x))
        : process.argv.indexOf(arg) !== -1;
}
