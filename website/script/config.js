const toggleConfiguration = function() {
    var input_trappen = document.querySelector(".js-quantity-toggle");
    var input_moeilijkheid = document.querySelector(".js-dropdown-toggle");
    var label_trappen = document.querySelector(".js-trappen-toggle");
    var label_moeilijkheid = document.querySelector(".js-moeilijkheid-toggle");

    var toggle_trappen = document.querySelector(".js-toggle-trappen");
    var toggle_moeilijkheid = document.querySelector(".js-toggle-moeilijkheid");
    var btn_start = document.querySelector(".js-startspel");
    var btn_volgende = document.querySelector(".js-volgendestap");


    if( toggle_trappen.style.display == "none" ) // scherm bij stap 2
    {
    //    input_trappen.style.display = "flex";
    //    input_moeilijkheid.style.display = "none";
    //    label_trappen.style.display = "none";
    //    label_moeilijkheid.style.display = "block";

        toggle_trappen.style.display = "block";
        toggle_moeilijkheid.style.display = "none";
        btn_start.style.display = "none";
        btn_volgende.style.display = "flex";
    }
    else
    {
        // input_trappen.style.display = "none";
        // input_moeilijkheid.style.display = "block";
        // label_trappen.style.display = "block";
        // label_moeilijkheid.style.display = "none";
        toggle_trappen.style.display = "none";
        toggle_moeilijkheid.style.display = "block";
        btn_start.style.display = "flex";
        btn_volgende.style.display = "none";
    }
 };

document.addEventListener('DOMContentLoaded', function() {
    const btn_vorigestap = document.querySelector(".js-vorigestap");
    btn_vorigestap.addEventListener('click', event => {
        toggleConfiguration();
    });

    const btn_volgendestap = document.querySelector(".js-volgendestap");
    btn_volgendestap.addEventListener('click', event => {
        toggleConfiguration();
    });
});