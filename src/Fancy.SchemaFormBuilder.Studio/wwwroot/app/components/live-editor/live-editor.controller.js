﻿(function () {
    "use strict";

    angular.module("fancy.schema-form-builder-studio")
        .controller("LiveEditorController", ["$http", "$timeout", function ($http, $timeout) {

            var localDtoStorageKeyPrefix = "ShemaFormBuilderStudio_";

            var vm = this;
            vm.showDtoExplorer = false;
            vm.compileAndRenderTimeout = null;

            loadLocalDtoNames();

            // Get all example names
            $http.get("/api/examples").success(function (exampleNames) {
                vm.exampleNames = exampleNames;
                vm.selectedExample = exampleNames[0];
                vm.loadExample();
            });

            // Loads an example dto from the server into editor
            vm.loadExample = function () {
                $http.get("/api/examples/" + vm.selectedExample).success(function (exampleCode) {
                    vm.code = exampleCode;
                });
            };

            // Sets a timeout and send code to server
            vm.onCodeChanged = function () {
                compileAndRenderByTimeout();
            };

            // Saves a dto into the local browser storage
            vm.saveLocalDto = function () {
                localStorage.setItem(localDtoStorageKeyPrefix + vm.dtoName, vm.code);
                loadLocalDtoNames();
            };

            // Loads a dto into the editor from the local browser storage
            vm.loadLocalDto = function (key) {
                vm.code = localStorage.getItem(localDtoStorageKeyPrefix + key);
                vm.selectedExample = null;
                vm.dtoName = key;
            };

            // Deletes a dto form the local browser storage
            vm.deleteLocalDto = function (key) {
                localStorage.removeItem(localDtoStorageKeyPrefix + key);
                loadLocalDtoNames();
            };

            // Load all locally saved dto names
            function loadLocalDtoNames() {
                vm.localDtoNames = new Array();
                for (var key in localStorage) {
                    // All items with the key prefix belog to us
                    if (key.substring(0, localDtoStorageKeyPrefix.length) === localDtoStorageKeyPrefix) {
                        vm.localDtoNames.push(key.substring(localDtoStorageKeyPrefix.length, key.length));
                    }
                }
            }

            // Wait for changes to copile and render the form
            function compileAndRenderByTimeout() {
                // Cancel an existing timeout
                if (vm.compileAndRenderTimeout) {
                    $timeout.cancel(vm.compileAndRenderTimeout);
                }

                // Set a new timeout
                vm.compileAndRenderTimeout = $timeout(compileAndRender, 1000);
            };

            // Sends code in editor to server to compile it
            function compileAndRender() {
                vm.compileAndRenderTimeout = null;
                // Send editor content to backend to compile the C# code
                $http.put("/api/forms/code", { csharpCode: vm.code }).success(function (compileResult) {
                    // Set up the resulting form description to render it
                    vm.compileResult = compileResult;
                });
            };

        }]);

})();