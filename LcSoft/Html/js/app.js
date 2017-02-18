var app = angular.module('lcApp', ['ngRoute', 'ui.bootstrap']);

app.config(function ($routeProvider, $locationProvider) {
    $routeProvider
        .when('/', {
            //controller: 'HomeController',
            contrllerAs: 'home',
            templateUrl: './View/Login.html',
            caseInsensitiveMatch: true //解决地址栏大小写问题
        })
        .when('/Login', {
            //controller: 'LoginController',
            contrllerAs: 'login',
            templateUrl: './View/Login.html',
            caseInsensitiveMatch: true
        })
        .when('/about', {
            //controller: 'AboutController',
            templateUrl: 'embedded.about.html',
            caseInsensitiveMatch: true
        })
        //-------------------------------------------Dict Begin----------------------------------------------
        .when('/Dict/blood/', {
            controller: 'DictBloodListController',
            templateUrl: './View/Dict/Blood/List.html',
            caseInsensitiveMatch: true
        })
        .when('/Dict/blood/:id', {
            controller: 'DictBloodEditController',
            templateUrl: './View/Dict/Blood/Edit.html',
            caseInsensitiveMatch: true
        })
        //-------------------------------------------Dict End----------------------------------------------
        .otherwise({
            redirectTo: '/home'
        })

    // use the HTML5 History API
    //$locationProvider.html5Mode(true);
})