﻿'use strict';

(function () {

    // Declare app level module which depends on filters, and services
    var app = angular.module('songApp', [
      , 'ngRoute'
      , 'eisnel.shared'
    ])

    .config(['$routeProvider', function ($routeProvider) {

        $routeProvider.when('/songs/list', { templateUrl: 'partials/partial1.html', controller: 'MyCtrl1' });
        $routeProvider.when('/song/search', { templateUrl: 'partials/partial2.html', controller: 'MyCtrl2' });
        $routeProvider.otherwise({ redirectTo: '/view1' });
    }])

    .factory('Song', ['$resource', function ($resource) {

        //var apiKey = "testKey";

        var Song = $resource("http://tinysong.com/s/:searchTerm?format=json&limit=7&key=:key",
            {
                searchTerm: "@searchTerm",
                key: '@key'
            },
            {
                search: {
                    method: 'GET',
                    params: { searchTerm: '@searchTerm', key: '@key' },
                    isArray: true
                }
            }
        );

        return Song;
    }])

    .controller('SongSearchCtrl', ['$scope', 'Song', 'dpNotify', function ($scope, Song, dpNotify) {

        $scope.variable = 'World';

        $scope.searchTerm;

        $scope.search = function () {

            if (!$.trim($scope.searchTerm).length) {
                dpNotify.info("Blank search: " + $scope.searchTerm, "Search clicked");
                // $scope.searchResults = [];
                $scope.searchResults = [
                { 'SongName': 'Thriller', 'ArtistName': 'Michael Jackson' }
                ];
                $scope.searchResultsJson = angular.toJson($scope.searchResults, true);
            }
            else {
                dpNotify.info("Starting search: " + $scope.searchTerm, "Search clicked");

                $scope.searchInProgress = true; // show the spinner.
                $scope.searchResults = Song.search(
                {
                    searchTerm: window.encodeURIComponent($scope.searchTerm),
                    key: $scope.tinySongApiKey
                },
                function (result) {
                    $scope.searchInProgress = false; // hide the spinner.
                }
                );
            }
        }

    }])

    .controller('SongController', ['$scope', 'dpNotify', function ($scope, dpNotify) {
  
        $scope.name = 'World';

        $scope.test = function() {
            dpNotify.success("This is only a test.", "Test");
            //alert("test!");
        }

        $scope.songs = [
		    {
		        "songId": "27048450"
			    , "title": "Total Eclipse of the Heart"
			    , "artist": "Bonnie Tyler"
			    , "likes": "8"
			    , "likedByUser": "true"
			    , "creator": { "userId": "1", "name": "Jane Roe" }
			    , "creationDate": "2014-05-09T17:57:28.556094Z"
			    , "url": "http://tinysong.com/zHNs"
		    }
		    , {
		        "songId": "33033285"
			    , "title": "We Found Love (Feat. Calvin Harris)"
			    , "artist": "Rihanna"
			    , "likes": "2"
			    , "likedByUser": "false"
			    , "creator": { "userId": "2", "name": "John Doe" }
			    , "creationDate": "2014-05-10T11:33:09.556094Z"
			    , "url": "http://tinysong.com/JSfq"
		    }
		    , {
		        "songId": "39364984"
			    , "title": "Blurred Lines (feat. T.I. and Pharrell Williams)"
			    , "artist": "Robin Thicke"
			    , "likes": "3"
			    , "likedByUser": "true"
			    , "creator": { "userId": "1", "name": "Jane Roe" }
			    , "creationDate": "2014-05-10T13:10:29.556094Z"
			    , "url": "http://tinysong.com/1eslM"
		    }
		    , {
		        "songId": "23570988"
			    , "title": "Hypnotize"
			    , "artist": "The Notorious B.I.G."
			    , "likes": "1"
			    , "likedByUser": "false"
			    , "creator": { "userId": "1", "name": "Jane Roe" }
			    , "creationDate": "2014-05-11T19:10:29.556094Z"
			    , "url": "http://tinysong.com/taZO"
		    }
        ];
    }])

    ;

})();