var app = angular.module('zbxlld');

app.controller('navbarController', [ '$scope', '$location', '$http', function($scope, $location, $http) {
    $scope.isView = function(path) {
        return path === $location.path();
    };
    
    $http.get('data/download.json')
    .success(function(result) {
        $scope.downloads = result;
    }).error(function(data) {
        console.log(data);
    });
}]);