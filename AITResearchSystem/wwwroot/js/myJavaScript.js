
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


// when click in a visible row it display the second
// row of the pair with more information about the 
// respondent answers in Admin page
$("#thetable tbody tr:even").on('click',
    function () {
        console.log("clicked in a row");
        var row2 = $(this).closest('tr').next('tr');
        console.log(row2.index());
        if ($(row2).attr("value") === "false") {
            console.log("true");
            $(row2).css("display", "");
            $(row2).attr("value","true");
        } else {
            console.log("false");
            $(row2).css("display", "none");
            $(row2).attr("value", "false");
        }
    });

// every respondent answers are a pair of two rows.
// when the page loads it hides the second row of the pair
$(document).ready(function () {
    $("[name=\"tr-hidden\"]").css("display", "none");
    $("[name=\"tr-hidden\"]").attr("value","false");
    console.log("added initial css and value");
});