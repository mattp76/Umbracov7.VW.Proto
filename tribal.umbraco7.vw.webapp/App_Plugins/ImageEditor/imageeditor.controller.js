angular.module("umbraco")
    .controller("My.ImageEditorController",
    function ($scope, assetsService) {
        $scope.setExtImage = function () {

            setTimeout(function () { createImagePreview($scope.model.value, $scope.model.alias) }, 1000);
        }

        assetsService.load(['/App_Plugins/ImageEditor/imageeditor.js']).then(function () {

            $scope.loaded = true;
            if ($scope.model.value) {

                console.log('$scope.model.value', $scope.model.value);

                $scope.setExtImage();

            }
        });

        assetsService.loadCss("~/App_Plugins/ImageEditor/imageeditor.css");

    });