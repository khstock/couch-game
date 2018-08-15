var var1 = document.getElementById("roomID");
var var2 = document.getElementById("userName");

function submitClick(){
  var rID = var1.value;
  var uName = var2.value;
  var firebaseRef = firebase.database().ref();
  firebaseRef.child(rID).child("Players").child("player1").child("name").set(uName);
}
