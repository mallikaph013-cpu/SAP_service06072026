// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
	function showToasts() {
		const toastElements = document.querySelectorAll('.app-toast');
		toastElements.forEach(toastElement => {
			if (window.bootstrap && bootstrap.Toast) {
				const toast = bootstrap.Toast.getOrCreateInstance(toastElement);
				toast.show();
			}
		});
	}

	if (document.readyState === 'loading') {
		document.addEventListener('DOMContentLoaded', showToasts);
	} else {
		showToasts();
	}
})();
