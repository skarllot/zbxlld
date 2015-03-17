var app = angular.module('zbxlld', [ 'ngRoute', 'ui.bootstrap' ]);

app.config([ '$routeProvider', '$controllerProvider',
            function($routeProvider, $controllerProvider) {
    // Lazy loading
    app.registerCtrl = $controllerProvider.register;
    app.resolveScriptDeps = function(dependencies) {
        return ['$q', '$rootScope', function($q, $rootScope) {
            var deferred = $q.defer();
            $script(dependencies, function() {
                $rootScope.$apply(function() {
                    deferred.resolve();
                });
            });
            
            return deferred.promise;
        }];
    };
    // ------------

    $routeProvider
    .when('/', {
        templateUrl: 'view/home.html'
    })
    .when('/documentation/:level1', {
        templateUrl: 'view/documentation.html',
        resolve: { deps: app.resolveScriptDeps(['js/documentation.js'])}
    })
    .when('/documentation', {
        templateUrl: 'view/documentation.html',
        resolve: { deps: app.resolveScriptDeps(['js/documentation.js'])}
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