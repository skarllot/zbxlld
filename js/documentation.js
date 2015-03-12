var app = angular.module('zbxlld');

app.controller('docController', [ '$scope', '$http', function($scope, $http) {
    $scope.curVersion = 0;
    $scope.docsVersion = undefined;
    
    $http.get('data/docsversion.json')
    .success(function(result) {
        $scope.docsVersion = result;
    }).error(function(data) {
        console.log(data);
    });
    $scope.getVersion = function() {
        if ($scope.docsVersion) {
            return $scope.docsVersion[$scope.curVersion].version
        } else {
            return ""
        }
    };
    $scope.setVersion = function(index) {
        $scope.curVersion = index
    };
}]);