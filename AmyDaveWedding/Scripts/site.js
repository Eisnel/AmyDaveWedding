// Custom JavaScript

$(function () {
    
    // Setup the navbar's affix.
    // It should remain static and scroll until the Jumbotron scrolls off the window,
    // and then affix itself to the top of the window.
    $(document).ready(function () {

        var $navbar = $(".navbar-affixed");
        var $wrapper = $('.navbar-wrapper'); // the wrapper doesn't get changed to fixed positioning.
        var navbarHeight = $navbar.height();
        //var navbarOffsetTop = $wrapper.offset().top;

        //window.console && console.log('Affix: nav top=' + navbarOffsetTop + ', height=' + navbarHeight);

        // Set the wrapper's height to the navbar's height
        // so that when the navbar is removed (fixed positioning)
        // the wrapper will keep its height.
        // This will prevent content below the navbar
        // from "jumping up" when the affix occurs.
        $wrapper.height(navbarHeight);

        $navbar.affix({
            offset: {
                top: function () {
                    //window.console && console.log('affix->offset->top function called.');
                    return $wrapper.offset().top;
                }
            }
        });
    });

    $('#ext-conf-form .select-invitee .invitee-list a.btn-primary').click(function () {
        $this = $(this);
        console.log('click: this:');
        console.log(this);
        console.log($this);
        var inviteeId = $this.attr('data-invitee-id');
        $('#ext-conf-form input[name=InviteeId]').val(inviteeId);
        // toastr.info('click: inviteeId: ' + inviteeId);
        $('#ext-conf-form').submit();
    });
    
});
