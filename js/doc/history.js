app.registerCtrl('docHistoryController', [ '$rootScope', '$scope', '$http',
                                          function($rootScope, $scope, $http) {

    $rootScope.title = 'History — ' + $rootScope.title;

    $http.get('data/history.json')
    .success(function(result) {
        $scope.history = result;
    }).error(function(data) {
        console.log(data);
    });
}]);
