var app = angular.module('zbxlld');

app.controller('docController', [ '$scope', '$http', function($scope, $http) {
    $scope.curVersion = undefined;
    
    $http.get('data/docsversion.json')
    .success(function(result) {
        $scope.docsVersion = result;
        $scope.curVersion = result[0];
    }).error(function(data) {
        console.log(data);
    });
}]);