/**
 * VoyaBus Main Script
 * Handles global AJAX setup and common UI interactions
 */

$(document).ready(function () {
    // Log initialization
    console.log("VoyaBus Design System Initialized");

    // Generic AJAX handler for future use
    window.voyaAjax = function(url, method, data, successCallback, errorCallback) {
        $.ajax({
            url: url,
            type: method || 'GET',
            data: data,
            success: function(response) {
                if (successCallback) successCallback(response);
            },
            error: function(xhr, status, error) {
                console.error("VoyaBus AJAX Error: ", error);
                if (errorCallback) errorCallback(xhr, status, error);
            }
        });
    };

    // Tooltip and Popover initialization (Bootstrap)
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });
});
