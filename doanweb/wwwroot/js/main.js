/*  ---------------------------------------------------
  Template Name: Gym
  Description:  Gym Fitness HTML Template
  Author: Colorlib
  Author URI: https://colorlib.com
  Version: 1.0
  Created: Colorlib
---------------------------------------------------------  */

(function ($) {
    "use strict";

    // Preloder
    $(window).on('load', function () {
        $(".loader").fadeOut();
        $("#preloder").delay(400).fadeOut("slow");
    });

    /*------------------
        Navigation
    --------------------*/
    $(".canvas-open").on('click', function () {
        $(".offcanvas-menu-wrapper").addClass("show-offcanvas-menu-wrapper");
        $(".offcanvas-menu-overlay").addClass("active");
    });

    $(".canvas-close, .offcanvas-menu-overlay").on('click', function () {
        $(".offcanvas-menu-wrapper").removeClass("show-offcanvas-menu-wrapper");
        $(".offcanvas-menu-overlay").removeClass("active");
    });

    /*------------------
        Hero Slider
    --------------------*/
    $(".hs-slider").owlCarousel({
        loop: true,
        margin: 0,
        items: 1,
        dots: false,
        animateOut: 'fadeOut',
        animateIn: 'fadeIn',
        nav: true,
        navText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
        autoplay: true,
        autoplayTimeout: 8000,
    });

    /*------------------
        Team Slider
    --------------------*/
    if ($(".ts-slider").length > 0) {
        $(".ts-slider").owlCarousel({
            loop: true,
            margin: 0,
            items: 3,
            dots: true,
            nav: false,
            autoplay: false,
            autoplayTimeout: 8000,
            responsive: {
                0: {
                    items: 1,
                },
                768: {
                    items: 2,
                },
                992: {
                    items: 3,
                }
            }
        });
    }

    /*------------------
        Testimonial Slider
    --------------------*/
    if ($(".ts_slider").length > 0) {
        $(".ts_slider").owlCarousel({
            loop: true,
            margin: 0,
            items: 1,
            dots: false,
            nav: true,
            navText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
            autoplay: false,
            autoplayTimeout: 8000,
        });
    }

    /*------------------
        Search Switch
    --------------------*/
    $(".search-switch").on('click', function () {
        $(".search-model").fadeIn(400);
    });

    $(".search-close-switch").on('click', function () {
        $(".search-model").fadeOut(400);
    });

    /*------------------
        Slicknav
    --------------------*/
    $('.canvas-menu').slicknav({
        prependTo: '.offcanvas-menu-wrapper .canvas-menu',
        closedSymbol: '<i class="fa fa-angle-right"></i>',
        openedSymbol: '<i class="fa fa-angle-down"></i>',
    });

    /*------------------
        Gallery
    --------------------*/
    if ($(".gallery").length > 0) {
        var containerEl = document.querySelector(".gallery");
        if (typeof mixitup !== 'undefined') {
            var mixer = mixitup(containerEl);
        }
    }

    $(".magnific-popup").magnificPopup({
        type: 'image',
        gallery: {
            enabled: true
        }
    });

    /*------------------
        BarFiller
    --------------------*/
    if ($(".barfiller").length > 0) {
        $(".barfiller").barfiller();
    }

    /*------------------
        Set Background
    --------------------*/
    $(".set-bg").each(function () {
        var bg = $(this).data("setbg");
        $(this).css("background-image", "url(" + bg + ")");
    });

})(jQuery);