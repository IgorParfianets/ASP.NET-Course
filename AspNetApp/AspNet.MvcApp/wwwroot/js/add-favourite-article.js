//let state = false;

//function addArticle() {
//    if (state) {
//        document.getElementById("star").src = "~/image/star_enable.png";
//        state = false;
//    } else {
//        document.getElementById("star").src = "~/image/star_disable.png";
//        state = true;
//    }
//}




let state = false;
document.getElementById("favourite").onclick = function () {
    if (state) {
        document.getElementById("star").src = "/image/star_disable.png";
        state = false;
    } else {
        document.getElementById("star").src = "/image/star_funny.png";
        state = true;
    }
    
}
