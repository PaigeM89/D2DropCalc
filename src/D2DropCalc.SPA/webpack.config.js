// Note this only includes basic configuration for development mode.
// For a more comprehensive configuration check:
// https://github.com/fable-compiler/webpack-config-template

var path = require("path");
var HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const ReactRefreshWebpackPlugin = require('@pmmmwh/react-refresh-webpack-plugin');


// If we're running webpack-dev-server, assume we're in development
var isProduction = !hasArg(/webpack-dev-server/);
const isDevelopment = !isProduction;
var outputWebpackStatsAsJson = hasArg('--json');

if (!outputWebpackStatsAsJson) {
    console.log("Bundling CLIENT for " + (isProduction ? "production" : "development") + "...");
}

var CONFIG = {
    // The tags to include the generated JS and CSS will be automatically injected in the HTML template
    // See https://github.com/jantimon/html-webpack-plugin
    indexHtmlTemplate: "./src/index.html",
    fsharpEntry: "./src/App.fs.js",
    sassEntry: './src/scss/main.scss',
    outputDir: "./dist",
    assetsDir: "./public",
    // Use babel-preset-env to generate JS compatible with most-used browsers.
    // More info at https://babeljs.io/docs/en/next/babel-preset-env.html
    babel: {
        plugins: [process.env.FAST_REFRESH && isDevelopment && require.resolve('react-refresh/babel')].filter(Boolean),
        presets: ["@babel/preset-env", "@babel/preset-react"]
    },
    // When using webpack-dev-server, you may need to redirect some calls
    // to a external API server. See https://webpack.js.org/configuration/dev-server/#devserver-proxy
    devServerProxy: {
        '/api': {
            target: "http://localhost:5000",
            changeOrigin: true
        }
    }
}

// The HtmlWebpackPlugin allows us to use a template for the index.html page
// and automatically injects <script> or <link> tags for generated bundles.
var commonPlugins = [
    new HtmlWebpackPlugin({
        filename: 'index.html',
        template: resolve(CONFIG.indexHtmlTemplate),
        hash: true
    })
];

module.exports = {
    plugins: [new MiniCssExtractPlugin()],
    mode: "development",
    // entry: {
    //     demo: [
    //         resolve(CONFIG.fsharpEntry),
    //         resolve(CONFIG.sassEntry)
    //     ]
    // },

    entry: isProduction ? // We don't use the same entry for dev and production, to make HMR over style quicker for dev env
        {
            demo: [
                resolve(CONFIG.fsharpEntry),
                resolve(CONFIG.sassEntry)
            ]
        } : {
            app: [
                resolve(CONFIG.fsharpEntry),
            ],
            style: [
                resolve(CONFIG.sassEntry)
            ]
        },
    // Add a hash to the output file name in production
    // to prevent browser caching if code changes
    output: {
        path: resolve(CONFIG.outputDir),
        filename: isProduction ? '[name].[hash].js' : '[name].js'
    },
    devServer: {
        publicPath: "/",
        contentBase: "./public",
        port: 8090,
        hot: true,
        inline: true,
        proxy: CONFIG.devServerProxy
    },
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: "fable-loader"
        },
        {
            test: /\.(js|jsx)$/,
            exclude: /node_modules/,
            use: {
                loader: 'babel-loader',
                options: CONFIG.babel
            },
        },
        {
            test: /\.(sass|scss)$/,
            use: [
                isProduction
                    ? MiniCssExtractPlugin.loader
                    : 'style-loader',
                'css-loader',
                'sass-loader',
            ],
        },
        {
            test: /\.css$/i,
            use: ["style-loader", "css-loader"],
        },
        {
            test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*$|$)/,
            use: ["file-loader"]
        }
    ]},
    optimization: {
        splitChunks: {
            cacheGroups: {
                commons: {
                    test: /node_modules/,
                    name: "vendors",
                    chunks: "all"
                }
            }
        },
    },
    plugins: commonPlugins,
    // plugins: commonPlugins.concat([
    //     new ReactRefreshWebpackPlugin()
    // ]),
    resolve: {
        // See https://github.com/fable-compiler/Fable/issues/1490
        symlinks: false,
        modules: [resolve("./node_modules")],
        alias: {
            // Some old libraries still use an old specific version of core-js
            // Redirect the imports of these libraries to the newer core-js
            'core-js/es6': 'core-js/es'
        }
    },
}

function hasArg(arg) {
    return arg instanceof RegExp
        ? process.argv.some(x => arg.test(x))
        : process.argv.indexOf(arg) !== -1;
}

function resolve(filePath) {
    return path.isAbsolute(filePath) ? filePath : path.join(__dirname, filePath);
}
