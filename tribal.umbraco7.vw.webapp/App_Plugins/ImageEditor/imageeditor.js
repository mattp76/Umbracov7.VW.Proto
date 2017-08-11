var createImagePreview = function (url, alias) {

    var element = document.getElementById("image-editor_" + alias);
    if (element) {
        element.parentNode.removeChild(element);
    }

    var elem = document.createElement("img");
    elem.setAttribute("src", url);
    //elem.setAttribute("height", "200");
    //elem.setAttribute("width", "300");
    elem.setAttribute("class", "image-editor_" + alias);
    elem.setAttribute("id", "image-editor_" + alias);

    //setTimeout(function(){ 

    document.getElementById("placehere_" + alias).appendChild(elem);

    //}, 3000);
}