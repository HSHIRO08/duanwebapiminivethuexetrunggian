// Site JavaScript
$(document).ready(function () {
    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Smooth scroll
    $('a[href^="#"]').on('click', function (e) {
        e.preventDefault();
        var target = this.hash;
        if (target) {
            $('html, body').animate({
                scrollTop: $(target).offset().top - 70
            }, 500);
        }
    });

    // Form validation feedback
    $('form').on('submit', function () {
        var form = $(this);
        if (form[0].checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
        }
        form.addClass('was-validated');
    });

    // Image error handler
    $('img').on('error', function () {
        $(this).attr('src', 'data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" width="400" height="300"><rect fill="%23ccc" width="400" height="300"/><text fill="%23999" x="50%" y="50%" text-anchor="middle" dy=".3em">No Image</text></svg>');
    });

    // Tooltip initialization
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Popover initialization
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Number formatting
    $('.format-currency').each(function () {
        var value = parseFloat($(this).text());
        if (!isNaN(value)) {
            $(this).text(value.toLocaleString('vi-VN') + ' ?');
        }
    });

    // Confirm dialog for delete actions
    $('[data-confirm]').on('click', function (e) {
        if (!confirm($(this).data('confirm'))) {
            e.preventDefault();
        }
    });

    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('#backToTop').fadeIn();
        } else {
            $('#backToTop').fadeOut();
        }
    });

    $('#backToTop').on('click', function () {
        $('html, body').animate({ scrollTop: 0 }, 600);
        return false;
    });

    // Search filter toggle
    $('#toggleFilters').on('click', function () {
        $('#filterPanel').slideToggle();
        $(this).find('i').toggleClass('fa-chevron-down fa-chevron-up');
    });

    // Price range slider (if implemented)
    if ($('#priceRange').length) {
        $('#priceRange').on('input', function () {
            $('#priceValue').text($(this).val().toLocaleString('vi-VN') + ' ?');
        });
    }

    // Date picker min date
    $('input[type="date"], input[type="datetime-local"]').each(function () {
        if (!$(this).attr('min')) {
            var today = new Date().toISOString().split('T')[0];
            $(this).attr('min', today);
        }
    });

    // Loading overlay
    $('.btn-submit').on('click', function () {
        var btn = $(this);
        btn.prop('disabled', true);
        btn.html('<span class="spinner-border spinner-border-sm me-2"></span>?ang x? lý...');
    });

    // Auto format phone number
    $('input[type="tel"]').on('input', function () {
        var value = $(this).val().replace(/\D/g, '');
        if (value.length > 10) {
            value = value.substring(0, 10);
        }
        $(this).val(value);
    });

    // Password strength indicator
    $('input[type="password"][name="password"]').on('input', function () {
        var password = $(this).val();
        var strength = 0;

        if (password.length >= 6) strength++;
        if (password.length >= 8) strength++;
        if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
        if (/\d/.test(password)) strength++;
        if (/[^a-zA-Z\d]/.test(password)) strength++;

        var strengthText = ['R?t y?u', 'Y?u', 'Trung bình', 'M?nh', 'R?t m?nh'];
        var strengthColor = ['danger', 'warning', 'info', 'primary', 'success'];

        if (password.length > 0) {
            $('#passwordStrength')
                .removeClass('d-none')
                .removeClass('text-danger text-warning text-info text-primary text-success')
                .addClass('text-' + strengthColor[strength])
                .text('?? m?nh: ' + strengthText[strength]);
        } else {
            $('#passwordStrength').addClass('d-none');
        }
    });

    // Confirm password validation
    $('input[name="confirmPassword"]').on('input', function () {
        var password = $('input[name="password"]').val();
        var confirmPassword = $(this).val();

        if (confirmPassword.length > 0) {
            if (password === confirmPassword) {
                $(this).removeClass('is-invalid').addClass('is-valid');
            } else {
                $(this).removeClass('is-valid').addClass('is-invalid');
            }
        } else {
            $(this).removeClass('is-valid is-invalid');
        }
    });

    // Image preview
    $('input[type="file"]').on('change', function (e) {
        var file = e.target.files[0];
        var reader = new FileReader();
        var preview = $(this).data('preview');

        if (file && preview) {
            reader.onload = function (e) {
                $(preview).attr('src', e.target.result);
            };
            reader.readAsDataURL(file);
        }
    });

    // Add fade-in animation to cards
    $('.card').each(function (index) {
        $(this).css('animation-delay', (index * 0.1) + 's');
        $(this).addClass('fade-in');
    });
});

// Helper functions
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

function formatDate(date) {
    return new Date(date).toLocaleDateString('vi-VN');
}

function showLoading() {
    $('body').append('<div class="loading-overlay"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>');
}

function hideLoading() {
    $('.loading-overlay').remove();
}

function showNotification(message, type = 'success') {
    var alertClass = 'alert-' + type;
    var icon = type === 'success' ? 'check-circle' : 'exclamation-circle';
    
    var alert = $('<div class="alert ' + alertClass + ' alert-dismissible fade show position-fixed top-0 end-0 m-3" role="alert" style="z-index: 9999;">' +
        '<i class="fas fa-' + icon + '"></i> ' + message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
        '</div>');
    
    $('body').append(alert);
    
    setTimeout(function () {
        alert.fadeOut('slow', function () {
            $(this).remove();
        });
    }, 5000);
}

// AJAX helper
function ajaxRequest(url, method, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: method,
        data: data,
        beforeSend: function () {
            showLoading();
        },
        complete: function () {
            hideLoading();
        },
        success: function (response) {
            if (successCallback) successCallback(response);
        },
        error: function (xhr, status, error) {
            if (errorCallback) {
                errorCallback(xhr, status, error);
            } else {
                showNotification('Có l?i x?y ra, vui lòng th? l?i!', 'danger');
            }
        }
    });
}
