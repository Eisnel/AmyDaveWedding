

$(document).ready(function () {

    var isMobile =  Modernizr.touch && Modernizr.mq('only all and (max-width: 640px)');
    // isMobile = true;

    //var fakeElement2 = {};
    //fakeElement2.constanants = 'b c d f g k l m n p q r s t v x z'.split(' ');
    //fakeElement2.vowels = 'a e i o u y'.split(' ');
    //fakeElement2.categories = 'html css php javascript'.split(' ');
    //fakeElement2.suffices = 'on ium ogen'.split(' ');
    //fakeElement2.titles = 'Project title 1, Project title 2, Project title 3, Project title 4, Project title 5, Project title 6, Project title 7, Project title 8'.split(',');

    //fakeElement2.texts1 = 'Phasellus eu tincidunt quam. Etiam tortor massa, mollis at ultricies eu, blandit eget libero. Phasellus eget dolor diam, at aliquet mi. Donec quis lectus.'.split('..');
    //fakeElement2.texts2 = 'Cursus sodales mattis. Morbi eros augue, viverra nec blandit eget lore vitae vestibul, hendrerit eget nisi.'.split('..');

    //fakeElement2.images = 'newest1 newest2 newest3 newest4 newest5 newest6 newest7 newest8'.split(' ');
    //fakeElement2.getRandom = function (property) {
    //    var values = fakeElement2[property];
    //    return values[Math.floor(Math.random() * values.length)];
    //};
    //fakeElement2.create = function (count) {
    //    var category = fakeElement2.getRandom('categories');
    //    image = fakeElement2.getRandom('images');
    //    title = fakeElement2.getRandom('titles');
    //    text1 = fakeElement2.getRandom('texts1');
    //    text2 = fakeElement2.getRandom('texts2');

    //    category = fakeElement2.getRandom('categories');
    //    className = 'element ' + category;
    //    set_width = $('#gallery-grid').find('.element:first').width();
    //    return '' +
    //        '<article class="' + category + ' span3">' +
    //        '<div class="thumbnail hover">' +
    //            '<img src="assets/example/' + image + '.jpg" alt=""/>' +
    //            '<div class="mask"></div>' +
    //            '<div class="caption">' +
    //                '<span class="ico_block">' +
    //                    '<a href="assets/example/' + image + '.jpg" class="ico_zoom prettyPhoto"><span></span></a>' +
    //                '</span>' +
    //            '</div>' +
    //        '</div>' +
    //        '</article>';
    //};
    //fakeElement2.getGroup = function (count) {
    //    var i = Math.ceil(count), newEls = '';
    //    while (i--) {
    //        newEls += fakeElement2.create(count);
    //    }
    //    return newEls;
    //};

    if (!isMobile) {
        //$('#nav').smoothScroll();
        $('.navbar').smoothScroll(800);
        $('.wedding .referral').smoothScroll(1200);
    }

    // prettyPhoto Initialization
    // $("a[rel^='prettyPhoto']").prettyPhoto();

    // Bootstrap ScrollSpy:
    function refreshScrollSpy()
    {
        $('[data-spy="scroll"]').each(function () {
            var $spy = $(this).scrollspy('refresh')
        });
    }
    $(window).resize(function () {
        refreshScrollSpy();
    });

    //// isotope settings
    //// cache container
    //var $container = $('#gallery-grid');
    //if ($container.length > 0)
    //{
    //    //
    //    // initialize isotope
    //    $container.isotope({
    //        // options...
    //        itemSelector: 'article',
    //        resizable: false,
    //        masonry: { columnWidth: $container.width() / 12 }
    //        //, layoutMode : 'fitRows'
    //    });
    //    //
    //    // update columnWidth on window resize
    //    $(window).smartresize(function () {
    //        $container.isotope({
    //            // update columnWidth to a percentage of container width
    //            masonry: { columnWidth: $container.width() / 12 }
    //        });
    //    });
    //    // filter items when filter link is clicked
    //    $('#filtrable a').click(function () {
    //        var selector = $(this).attr('data-filter');
    //        $container.isotope({ filter: selector });
    //        // mark current li
    //        $(this).parent().parent().find('.current').removeClass('current');
    //        $(this).parent().addClass('current');
    //        return false;
    //    });
    //    // add more items to portfolio
    //    $('.load-more-grid').click(function () {
    //        var count = $(this).attr('data-count');
    //        var $newEls = $(fakeElement2.getGroup(count));
    //        $container.isotope('insert', $newEls, function () {
    //            relocate();
    //        });
    //    });
    //    // //
    //    function relocate() {
    //        setTimeout("$('#gallery-grid').isotope('reLayout')", 1000);
    //        $('.prettyPhoto').prettyPhoto();
    //    }
    //    $(window).load(function () {
    //        relocate();
    //    });
    //    $(window).resize(function () {
    //        relocate();
    //    });
    //}

    /*
    |--------------------------------------------------------------------------
    |  fullwidth image
    |--------------------------------------------------------------------------
    */
    if ($('#homeFullScreen').length)
    {
        if (!isMobile) {
            var fullscreenImage = function () {
                var height = $(window).height();
                $('#homeFullScreen').css({ height: height + 60 });
                // $('#debugPanel').html("window height: " + height + " + 60 = " + (height + 60));
            }
            fullscreenImage();
            $(window).on("resize", function (e) {
                if ($('#home').length) {
                    fullscreenImage();
                }
            });
        }

        var updateDebugWithSize = function () {
            var width = $(window).width();
            var height = $(window).height();
            $('#debugPanel').html("width: " + width + ", height: " + height + " + 60 = " + (height + 60));
        }
        updateDebugWithSize();
        $(window).on("resize", function (e) {
            if ($('#home').length) {
                updateDebugWithSize();
            }
        });
    }
    $('#debugPanel').html("Mobile: " + isMobile);


    // Center tests:
    var centerFunction = function () {
        var windowWidth = $(window).width();
        var windowScrollLeft = $(window).scrollLeft();
        $('.centerWithJavaScript').each(function (index, element) {
            var $element = $(element);
            var outerWidth = $element.outerWidth();
            $element.css("left", Math.max(0, ((windowWidth - outerWidth) / 2) + windowScrollLeft) + "px");
           // $('#debugPanel').html("Window width: " + windowWidth + ", scrollLeft: " + windowScrollLeft + ", circle outerWidth: " + outerWidth);
        });
    }
    centerFunction();
    $(window).on("resize", centerFunction);



    $('.gifts .option .summary').click(function (e) {
        e.preventDefault();

        $('.gifts .option .detail').hide(500);
        $('.gifts .option .summary .show-detail').show();

        var $option = $(this).parents('.option');

        jQuery('.summary .show-detail', $option).hide();
        jQuery('.detail', $option).show(500);
    });

    $('.gifts .option .detail .hide-detail').click(function (e) {
        e.preventDefault();

        var $detail = $(this).parents('.detail');
        var $option = $(this).parents('.option');

        $detail.hide(500);
        jQuery('.summary .show-detail', $option).show();
    });


    $(".email-replace").text('eisnel' + '@' + 'ao' + 'l.c' + 'om');
});

//function fullscreenImage()
//{
//    var height = $(window).height();
//    $('#homeFullScreen').css({ height: height + 60 });
//    // $('#debugPanel').html("window height: " + height + " + 60 = " + (height + 60));
//}
//
//function updateDebugWithSize()
//{
//    $('#debugPanel').html("window height: " + height + " + 60 = " + (height + 60));
//}


