var app = angular.module('zbxlld', [ 'ngRoute', 'ui.bootstrap' ]);

app.config([ '$routeProvider', '$controllerProvider',
            function($routeProvider, $controllerProvider) {
    // Lazy loading
    app.registerCtrl = $controllerProvider.register;
    // ------------

    $routeProvider
    .when('/', {
        title: 'Home',
        templateUrl: 'view/home.html'
    })
    .when('/documentation/:level1', {
        title: 'Documentation',
        templateUrl: 'view/documentation.html'
    })
    .when('/documentation', {
        title: 'Documentation',
        redirectTo: '/documentation/index'
    })
    .otherwise({
        redirectTo: '/'
    });
}]);

app.run(['$location', '$rootScope', function($location, $rootScope) {
    $rootScope.$on('$routeChangeSuccess', function(event, current, previous) {
        $rootScope.title = current.$$route.title;
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