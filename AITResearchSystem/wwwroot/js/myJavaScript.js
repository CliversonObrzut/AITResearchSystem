
// function to correct position the footer in the page
// depending if the document page is higher than the visible screen
// it changes the css property position for the footer
function fixFooter() {
    console.log($(window).height());
    console.log($(document).height());
        if($(window).height() < $(document).height()) {
            $('.footer-basic').css("position", "relative");
        }
        else {
            $('.footer-basic').css("position", "fixed");
        }
}

// fix footer position when page resize
$(window).resize(function() {
    fixFooter();
});
// fix footer position when page loads and it is ready
$(document).ready(function () {
    fixFooter();
});

$(document).bind(function () {
    fixFooter();
});

// when click in a visible row it display the second
// row of the pair with more information about the 
// respondent answers in Admin page
$("#thetable tbody tr:even").on('click',
    function () {
        var row2 = $(this).closest('tr').next('tr');
        if ($(row2).attr("style") === "display:none") {
            $(row2).attr("style","");
        } else {
            $(row2).attr("style", "display:none");
        }
        fixFooter();
    });