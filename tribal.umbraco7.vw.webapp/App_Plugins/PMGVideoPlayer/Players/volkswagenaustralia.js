/**
 * @fileoverview YouTube Media Controller - Wrapper for YouTube Media API
 */

/**
 * YouTube Media Controller - Wrapper for YouTube Media API
 * @param {videojs.Player|Object} player
 * @param {Object=} options
 * @param {Function=} ready
 * @constructor
 */

console.log("VW loaded", videojs.MediaTechController);

videojs.VolkswagenAustralia = videojs.MediaTechController.extend({
    /** @constructor */

    init: function (player, options, ready) {

        console.log('TESTER!!!!!!!');

        videojs.MediaTechController.call(this, player, options, ready);

        // Save those for internal usage
        this.player_ = player;
        this.player_el_ = document.getElementById(player.id());
        this.player_el_.className += ' vjs-youtube';

        this.id_ = this.player_.id() + '_youtube_api';

        this.el_ = videojs.Component.prototype.createEl('iframe', {
            id: this.id_,
            className: 'vjs-tech',
            scrolling: 'no',
            marginWidth: 0,
            marginHeight: 0,
            frameBorder: 0,
            webkitAllowFullScreen: 'true',
            mozallowfullscreen: 'true',
            allowFullScreen: 'true'
        });

        // This makes sure the mousemove is not lost within the iframe
        // Only way to make sure the control bar shows when we come back in the video player
        this.iframeblocker = videojs.Component.prototype.createEl('div', {
            className: 'iframeblocker'
        });

        this.player_el_.insertBefore(this.iframeblocker, this.player_el_.firstChild);
        this.player_el_.insertBefore(this.el_, this.iframeblocker);

        console.log('player.options()[src]', player.options()['src']);

        this.parseSrc(player.options()['src']);

        // If we are not on a server, don't specify the origin (it will crash)
        if (window.location.protocol != 'file:') {
            //params.origin = window.location.protocol + '//' + window.location.host; https://www.youtube.com/watch?v=tVA57Z7yxj8
            //this.el_.src = window.location.protocol + '//www.youtube.com/embed/' + this.videoId + '?' + videojs.VolkswagenAustralia.makeQueryString(params);
            this.el_.src = this.srcVal;
        } else {
            this.el_.src = 'https://www.youtube.com/embed/' + this.videoId + '?' + videojs.VolkswagenAustralia.makeQueryString(params);
        }


        // If we are not on a server, don't specify the origin (it will crash)
        //this.el_.src = 'https://secure.volkswagenaustralia.com.au/vehicle-image/ro-crm--6c13fx-16--d7d7.png?youtube';

        //this.el_.src = this.srcVal;

        this.on('dispose', function () {
            // Get rid of the created DOM elements
            this.el_.parentNode.removeChild(this.el_);
            this.iframeblocker.parentNode.removeChild(this.iframeblocker);

            this.player_.loadingSpinner.hide();
            this.player_.bigPlayButton.hide();
        });
    }
});

videojs.VolkswagenAustralia.prototype.parseSrc = function (src) {
    this.srcVal = src;

    if (src) {
        // Regex to parse the video ID
        var regId = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*/;
        var match = src.match(regId);

        if (match && match[2].length == 11) {
            this.videoId = match[2];
        } else {
            this.videoId = null;
        }

        // Regex to parse the playlist ID
        var regPlaylist = /[?&]list=([^#\&\?]+)/;
        match = src.match(regPlaylist);

        if (match != null && match.length > 1) {
            this.playlistId = match[1];
        } else {
            // Make sure their is no playlist
            if (this.playlistId) {
                delete this.playlistId;
            }
        }

        // Parse video quality option
        var regVideoQuality = /[?&]vq=([^#\&\?]+)/;
        match = src.match(regVideoQuality);

        if (match != null && match.length > 1) {
            this.userQuality = match[1];
        }
    }
};

videojs.VolkswagenAustralia.prototype.src = function (src) {
    if (src) {
        this.parseSrc(src);

        if (this.videoId == null) {
            // Set the black background if the URL isn't valid
            this.iframeblocker.style.backgroundColor = 'black';
            this.iframeblocker.style.display = 'block';
        } else {
            this.ytplayer.loadVideoById({
                videoId: this.videoId,
                suggestedQuality: this.userQuality
            });

            // Update the poster
            this.player_el_.getElementsByClassName('vjs-poster')[0].style.backgroundImage = 'url(https://img.youtube.com/vi/' + this.videoId + '/0.jpg)';
            this.iframeblocker.style.backgroundColor = '';
            this.iframeblocker.style.display = '';
            this.player_.poster('https://img.youtube.com/vi/' + this.videoId + '/0.jpg');
        }
    }

    return this.srcVal;
};

videojs.VolkswagenAustralia.prototype.load = function () { };

//Cross browser solution to add text content to an element
function setInnerText(element, text) {
    var textProperty = ('innerText' in element) ? 'innerText' : 'textContent';
    element[textProperty] = text;
}


// Stretch the YouTube poster
// Keep the iframeblocker in front of the player when the user is inactive
// (ONLY way because the iframe is so selfish with events)
(function () {
    var style = document.createElement("style");
    style.type = 'text/css';
    var css = " .vjs-youtube .vjs-poster { background-size: cover; }.iframeblocker { display:none;position:absolute;top:0;left:0;width:100%;height:100%;cursor:pointer;z-index:2; }.vjs-youtube.vjs-user-inactive .iframeblocker { display:block; } .vjs-quality-button > div:first-child > span:first-child { position:relative;top:7px }";
    setInnerText(style, css);
    document.getElementsByTagName("head")[0].appendChild(style);
})();