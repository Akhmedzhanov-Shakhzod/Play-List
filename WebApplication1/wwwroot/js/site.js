// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



function playAudio(src) {
	//document.write(path);

	var sound = document.createElement('audio');
	sound.controls = 'controls';
	sound.src = src;
	sound.type = 'audio/mpeg';
	sound.autoplay = 'autoplay';
	$('#Player').empty();
	document.getElementById('Player').appendChild(sound);
	document.getElementById('Player').style.visibility = 'visible';
}
