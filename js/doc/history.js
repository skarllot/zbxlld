app.registerCtrl('docHistoryController', [ '$scope', '$http',
                                          function($scope, $http) {

    $http.get('data/history.json')
    .success(function(result) {
        $scope.history = result;
    }).error(function(data) {
        console.log(data);
    });
}]);
