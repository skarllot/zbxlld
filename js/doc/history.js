app.registerCtrl('docHistoryController', [ '$rootScope', '$scope', '$http',
                                          function($rootScope, $scope, $http) {

    $rootScope.title += '/History';
    $scope.loaded = false;

    $http.get('data/history.json')
    .success(function(result) {
        $scope.history = result;
        $scope.loaded = true;
    }).error(function(data) {
        console.log(data);
    });
}]);
