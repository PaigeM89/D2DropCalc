const path = require("path");

module.exports = {
  allFiles: true, // convert all files and not only entrypoint
  entry: path.join(__dirname, "./src/App.fsproj"),
  outDir: path.join(__dirname, "../D2DropCalc.Server/WebRoot/scripts/"),
  babel: { sourceMaps: "inline" } // enable sourceMaps to see F# code and not generated js in test reports
};