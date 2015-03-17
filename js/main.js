var app = angular.module('zbxlld', [ 'ngRoute', 'ui.bootstrap' ]);

app.config([ '$routeProvider', '$controllerProvider',
            function($routeProvider, $controllerProvider) {
    // Lazy loading
    app.registerCtrl = $controllerProvider.register;
    // ------------

    $routeProvider
    .when('/', {
        templateUrl: 'view/home.html'
    })
    .when('/documentation/:level1', {
        templateUrl: 'view/documentation.html'
    })
    .when('/documentation', {
        templateUrl: 'view/documentation.html'
    })
    .otherwise({
        redirectTo: '/'
    });
}]);

var getHashLength = function(hList) {
    if (Object.keys == undefined) {
        var count = 0;
        for (i in hList) count++;
        return count;
    } else {
        return Object.keys(hList).length;
    }
};