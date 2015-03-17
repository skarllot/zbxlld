//app.controller('docController', [ '$scope', '$http', '$routeParams', function($scope, $http, $routeParams) {
app.registerCtrl('docController', [ '$scope', '$http', '$routeParams',
                                   function($scope, $http, $routeParams) {
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
    
    $scope.subRoute = function() {
        switch ($routeParams.level1) {
            case 'history':
                //app.resolveScriptDeps(['js/doc/history.js']);
                return 'view/doc/history.html';
            default:
                return 'view/doc/index.html';
        };
    };
}]);