;(function (window, angular, undefined) {  'use strict';

    angular.module('sc')
        .constant('version', '1.2.0')
        .constant('scConfig', window.SC.config);

}(window, window.angular));