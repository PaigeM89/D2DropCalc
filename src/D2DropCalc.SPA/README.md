# Fable Minimal App

TODO:
0. make a single dropdown component
1. make output render components
2. recreate that scss error from zaid's dropdown package and try to fix?

1. I don't have the data where I need it to convert a (difficulty, drop quality, monster) to a treasure class.
2. I DO have a treasure class, I can invoke a callback on that in the meantime.
3. I SHOULD put all the core data into core state, fuck responsible size limits.

    a. This also lets me fine-tune how i display the active treasure classes
4. once i can build the calculation inputs, i can finally get back to testing the endpoint

This is a small Fable app project so you can easily get started and add your own code easily in it.

## Requirements

* [dotnet SDK](https://www.microsoft.com/net/download/core) 3.0 or higher
* [node.js](https://nodejs.org) with [npm](https://www.npmjs.com/)
* An F# editor like Visual Studio, Visual Studio Code with [Ionide](http://ionide.io/) or [JetBrains Rider](https://www.jetbrains.com/rider/).

## Building and running the app

* Install JS dependencies: `npm install`
* Install F# dependencies: `npm start`
* After the first compilation is finished, in your browser open: http://localhost:8080/

Any modification you do to the F# code will be reflected in the web page after saving.

## Project structure

### npm

JS dependencies are declared in `package.json`, while `package-lock.json` is a lock file automatically generated.

### Webpack

[Webpack](https://webpack.js.org) is a JS bundler with extensions, like a static dev server that enables hot reloading on code changes. Fable interacts with Webpack through the `fable-loader`. Configuration for Webpack is defined in the `webpack.config.js` file. Note this sample only includes basic Webpack configuration for development mode, if you want to see a more comprehensive configuration check the [Fable webpack-config-template](https://github.com/fable-compiler/webpack-config-template/blob/master/webpack.config.js).

### F#

The sample only contains two F# files: the project (.fsproj) and a source file (.fs) in the `src` folder.

### Web assets

The `index.html` file and other assets like an icon can be found in the `public` folder.
