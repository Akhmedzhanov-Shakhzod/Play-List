// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/*
document.getElementById("play")
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
*/


/*

document.querySelector('#elastic').oninput = function () {
	let val = this.value.trim();

	let elastinItmes = document.querySelectorAll('.elastic');

	if (val != '') {
		elastinItmes.forEach(function (elem) {

			if (elem.innerText.search(val) == -1) {
				elem.classList.add('hide');
			}
			else {
				elem.classList.remove('hide');

				let str = elem.innerHTML;
				elem.innerHTML = insertMark(str, elem.innerText.search(val), val.length)
			}
		})
	}
	else {
		elastinItmes.forEach(function (elem) {
			elem.classList.remove('hide');
		})
	}
}

function insertMark(string,pos,len) {

	return string.slice(0, pos) + '<mark>' + string.slice(pos, pos + len) + '</mark>' + string.slice(pos + len, string.length);
}
* /


*/

$(document).ready(function () {
	$('.multi-select').selectpicker();
});

