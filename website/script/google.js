var auth2;
var googleUser; // The current user

var signinChanged = function (val) {
  console.log("Signin state changed to ", val);
  if(val == true) {
    
    if (auth2.isSignedIn.get()) {
      var profile = auth2.currentUser.get().getBasicProfile();
      console.log('ID: ' + profile.getId());
      console.log('Full Name: ' + profile.getName());
      document.cookie = `name=${profile.getName()}`;
      document.cookie = `google=true`
      if(window.location.href != `http://glenntroncquo.be/main.html`) window.location.href= `http://glenntroncquo.be/main.html`;
    }
  }
};

var onSuccess = function (user) {
  console.log("Signed in as " + user.getBasicProfile().getName());
  // Redirect somewhere
};


function signOut() {
  auth2.signOut().then(function () {
    console.log("User signed out.");
    
  });
}

var userChanged = function (user) {
  if (user.getId()) {
    // Do something here
    console.log('user changed');
    var profile = auth2.currentUser.get().getBasicProfile();
    console.log('ID: ' + profile.getId());
    console.log('Full Name: ' + profile.getName());
  }
};




document.addEventListener("DOMContentLoaded", () => {


  gapi.load("auth2", function () {
    auth2 = gapi.auth2.init({
      client_id:
        "638448013795-1k1csiit9uc7ikjg8h4jp76ht3g09epg.apps.googleusercontent.com",
    });
  
    auth2.isSignedIn.listen(signinChanged);
    // auth2.currentUser.listen(userChanged); // This is what you use to listen for user changes

    

  });
});
