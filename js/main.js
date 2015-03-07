var app = angular.module('zbxlld', [ 'ngRoute', 'ui.bootstrap' ]);

app.config([ '$routeProvider', function($routeProvider) {
    $routeProvider
    .when('/', {
        templateUrl: 'view/home.html'
    })
    .when('/documentation', {
        controller: 'docController',
        templateUrl: 'view/documentation.html'
    })
    .otherwise({
        redirectTo: '/'
    });
}]);
