const toggleState = function() {
   var d1 = document.getElementById("js-geslotendiv");
   var d2 = document.getElementById("js-opendiv");
   if( d2.style.display == "none" )
   {
      d1.style.display = "none";
      d2.style.display = "flex";
   }
   else
   {
      d1.style.display = "flex";
      d2.style.display = "none";
   }
};

document.addEventListener('DOMContentLoaded', function() {
    console.log("DOM loaded")
    const btn_uitleg_gesloten = document.querySelector(".js-uitleg__gesloten");
    btn_uitleg_gesloten.addEventListener('click', event => {
        toggleState();
    });

    const btn_uitleg_open = document.querySelector(".js-uitleg__open");
    btn_uitleg_open.addEventListener('click', event => {
        toggleState();
    });
});