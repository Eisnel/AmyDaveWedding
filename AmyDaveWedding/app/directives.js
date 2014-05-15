'use strict';

/* Directives */


angular.module('eisnel.shared', [])

// on-blur-change directive from:
// http://nadeemkhedr.wordpress.com/2013/09/01/build-angularjs-grid-with-server-side-paging-sorting-filtering/
.directive('onBlurChange', function ($parse) {
    return function (scope, element, attr) {
        var fn = $parse(attr['onBlurChange']);
        var hasChanged = false;
        element.on('change', function (event) {
            hasChanged = true;
        });

        element.on('blur', function (event) {
            if (hasChanged) {
                scope.$apply(function () {
                    fn(scope, { $event: event });
                });
                hasChanged = false;
            }
        });
    };
})

// on-enter-blur directive from:
// http://nadeemkhedr.wordpress.com/2013/09/01/build-angularjs-grid-with-server-side-paging-sorting-filtering/
.directive('onEnterBlur', function () {
    return function(scope, element, attrs) {
        element.bind("keydown keypress", function(event) {
            if(event.which === 13) {
                element.blur();
                event.preventDefault();
            }
        });
    };
})

.factory('dpNotify', function () {

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "positionClass": "toast-bottom-right",
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "3000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "slideDown",
        "hideMethod": "fadeOut"
    };

    return {
        success: function (message, title) {
            toastr.success(message, title);
        },
        info: function (message, title) {
            toastr.info(message, title);
        },
        warning: function (message, title) {
            toastr.warning(message, title);
        },
        error: function (message, title) {
            toastr.error(message, title);
        }
    };
})

;