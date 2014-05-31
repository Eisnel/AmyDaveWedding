// Custom JavaScript

$(function () {
    
    $(document).ready(function () {

        toastr.info('Are you the 6 fingered man?');

        $(window).on("load resize", function (e) {

            var $navbar = $(".navbar-affixed");
            var navbarHeight = $navbar.height();
            var navbarOffsetTop = $navbar.offset().top;

            //toastr.info('nav top=' + navbarOffsetTop + ', height=' + navbarHeight + ', event: ' + e, 'Affix event');
            window.console && console.log('Affix event=' + e.type + ' nav top=' + navbarOffsetTop + ', height=' + navbarHeight);

            $('.navbar-wrapper').height(navbarHeight);

            $navbar.affix({
                offset: { top: navbarOffsetTop }
            });
        });

        

    });
    

});