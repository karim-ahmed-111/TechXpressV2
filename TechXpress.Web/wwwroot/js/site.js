// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// TechXpress Global Animation Effects

document.addEventListener('DOMContentLoaded', function() {
    // Initialize animations
    initParticleBackground();
    initMagicHovers();
    initScrollAnimations();
    initProductImageZoom();
    initGlowEffects();
    initStarRatings();
    manageLoadingScreen();
    
    // Handle navigation menu highlighting
    highlightCurrentNavItem();
});

/**
 * Creates interactive particle background
 */
function initParticleBackground() {
    const container = document.getElementById('particles-background');
    if (!container) return;
    
    // Clear any existing particles first
    container.innerHTML = '';
    
    // Create multiple layers of particles for depth effect
    createParticleLayer(container, 30, 1, 4, 0.1, 0.3, 15, 25);   // Small distant particles
    createParticleLayer(container, 15, 3, 7, 0.2, 0.5, 20, 40);   // Medium particles
    createParticleLayer(container, 8, 5, 10, 0.3, 0.6, 30, 50);   // Large close particles
}

/**
 * Creates a layer of animated particles
 */
function createParticleLayer(container, count, minSize, maxSize, minOpacity, maxOpacity, minDuration, maxDuration) {
    for (let i = 0; i < count; i++) {
        // Calculate random properties
        const size = Math.random() * (maxSize - minSize) + minSize;
        const posX = Math.random() * 100;
        const posY = Math.random() * 100;
        const opacity = Math.random() * (maxOpacity - minOpacity) + minOpacity;
        const duration = Math.random() * (maxDuration - minDuration) + minDuration;
        const delay = Math.random() * 10;
        
        // Create the particle element
        const particle = document.createElement('div');
        particle.classList.add('particle');
        
        // Apply styles
        particle.style.width = `${size}px`;
        particle.style.height = `${size}px`;
        particle.style.left = `${posX}%`;
        particle.style.top = `${posY}%`;
        particle.style.opacity = opacity;
        
        // Apply animations with random parameters
        particle.style.animation = `
            float ${duration}s ease-in-out ${delay}s infinite, 
            pulse ${duration/2}s ease-in-out ${delay}s infinite
        `;
        
        // Add to container
        container.appendChild(particle);
    }
}

/**
 * Adds hover effects to interactive elements
 */
function initMagicHovers() {
    // Add magnetic effect to buttons
    const buttons = document.querySelectorAll('.btn-primary, .btn-accent, .btn-glow');
    buttons.forEach(btn => {
        btn.addEventListener('mousemove', (e) => {
            const btnRect = btn.getBoundingClientRect();
            const x = e.clientX - btnRect.left;
            const y = e.clientY - btnRect.top;
            
            // Calculate distance from center
            const centerX = btnRect.width / 2;
            const centerY = btnRect.height / 2;
            
            // Apply subtle transform based on mouse position
            const moveX = (x - centerX) / 10;
            const moveY = (y - centerY) / 10;
            
            btn.style.transform = `translate(${moveX}px, ${moveY}px)`;
        });
        
        btn.addEventListener('mouseleave', () => {
            btn.style.transform = 'translate(0, 0)';
        });
    });
    
    // Add hover glow to cards
    const cards = document.querySelectorAll('.product-card, .category-card');
    cards.forEach(card => {
        card.addEventListener('mouseenter', () => {
            card.classList.add('hover-active');
        });
        card.addEventListener('mouseleave', () => {
            card.classList.remove('hover-active');
        });
    });
}

/**
 * Adds scroll-triggered animations to elements
 */
function initScrollAnimations() {
    // Identify elements to animate on scroll
    const animatedElements = document.querySelectorAll('.section-title, .feature-card, .product-card, .hero-section, .category-card');
    
    // Create observer
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('scroll-animate-in');
                // Unobserve after animation to improve performance
                observer.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.1,  // Trigger when 10% of element is visible
        rootMargin: '0px 0px -50px 0px'  // Adjust when animation starts
    });
    
    // Observe each element
    animatedElements.forEach(el => {
        el.classList.add('scroll-animate');
        observer.observe(el);
    });
}

/**
 * Adds image zoom effect on product detail pages
 */
function initProductImageZoom() {
    const productImage = document.querySelector('.product-detail-image img');
    if (!productImage) return;
    
    const imageContainer = productImage.parentElement;
    
    imageContainer.addEventListener('mousemove', (e) => {
        const rect = imageContainer.getBoundingClientRect();
        const x = (e.clientX - rect.left) / rect.width;
        const y = (e.clientY - rect.top) / rect.height;
        
        const zoomFactor = 1.5;
        const offsetX = (0.5 - x) * 20;
        const offsetY = (0.5 - y) * 20;
        
        productImage.style.transform = `scale(${zoomFactor}) translate(${offsetX}px, ${offsetY}px)`;
    });
    
    imageContainer.addEventListener('mouseleave', () => {
        productImage.style.transform = 'scale(1) translate(0, 0)';
    });
}

/**
 * Adds glow effects to various elements
 */
function initGlowEffects() {
    // Star ratings hover effect
    const starContainers = document.querySelectorAll('.star-rating');
    starContainers.forEach(container => {
        const stars = container.querySelectorAll('i');
        
        stars.forEach((star, index) => {
            star.addEventListener('mouseenter', () => {
                // Fill this star and all previous stars
                stars.forEach((s, i) => {
                    if (i <= index) {
                        s.classList.add('hover');
                    } else {
                        s.classList.remove('hover');
                    }
                });
            });
        });
        
        container.addEventListener('mouseleave', () => {
            stars.forEach(s => s.classList.remove('hover'));
        });
    });
    
    // Cursor glow trail effect
    const cursorTrail = document.createElement('div');
    cursorTrail.classList.add('cursor-trail');
    document.body.appendChild(cursorTrail);
    
    document.addEventListener('mousemove', (e) => {
        // Get mouse position
        const mouseX = e.clientX;
        const mouseY = e.clientY;
        
        // Position the trail with a slight delay for trailing effect
        setTimeout(() => {
            cursorTrail.style.left = mouseX + 'px';
            cursorTrail.style.top = mouseY + 'px';
        }, 50);
    });
}

/**
 * Highlights the current nav item based on URL
 */
function highlightCurrentNavItem() {
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
    
    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        
        // Check if this link matches the current path
        if (href === currentPath || 
            (href !== '/' && currentPath.startsWith(href))) {
            link.classList.add('active');
        }
    });
}

// Shopping cart quantity handling
document.addEventListener('DOMContentLoaded', function() {
    const quantityControls = document.querySelectorAll('.quantity-control');
    
    quantityControls.forEach(control => {
        const decreaseBtn = control.querySelector('.decrease');
        const increaseBtn = control.querySelector('.increase');
        const input = control.querySelector('input');
        
        if (decreaseBtn && increaseBtn && input) {
            decreaseBtn.addEventListener('click', () => {
                const currentValue = parseInt(input.value) || 1;
                if (currentValue > 1) {
                    input.value = currentValue - 1;
                    // Trigger change event for any listeners
                    input.dispatchEvent(new Event('change'));
                }
            });
            
            increaseBtn.addEventListener('click', () => {
                const currentValue = parseInt(input.value) || 1;
                const max = parseInt(input.getAttribute('max')) || 99;
                
                if (currentValue < max) {
                    input.value = currentValue + 1;
                    // Trigger change event for any listeners
                    input.dispatchEvent(new Event('change'));
                } else {
                    // Shake effect when max reached
                    control.classList.add('shake');
                    setTimeout(() => {
                        control.classList.remove('shake');
                    }, 500);
                }
            });
        }
    });
});

// Add CSS rules dynamically
document.addEventListener('DOMContentLoaded', function() {
    const style = document.createElement('style');
    style.textContent = `
        /* Cursor trail effect */
        .cursor-trail {
            position: fixed;
            width: 30px;
            height: 30px;
            border-radius: 50%;
            background: radial-gradient(circle, rgba(99, 102, 241, 0.5) 0%, rgba(236, 72, 153, 0.1) 70%, transparent 100%);
            pointer-events: none;
            mix-blend-mode: screen;
            z-index: 9999;
            transform: translate(-50%, -50%);
            opacity: 0.7;
            transition: left 0.2s ease-out, top 0.2s ease-out;
            filter: blur(2px);
        }
        
        /* Scroll animations */
        .scroll-animate {
            opacity: 0;
            transform: translateY(30px);
            transition: opacity 0.8s ease, transform 0.8s ease;
        }
        
        .scroll-animate-in {
            opacity: 1;
            transform: translateY(0);
        }
        
        /* Card hover effects */
        .product-card.hover-active {
            transform: translateY(-15px) scale(1.03);
            z-index: 10;
            box-shadow: 0 20px 40px rgba(99, 102, 241, 0.4);
        }
        
        /* Shake animation for quantity inputs */
        @keyframes shake {
            0%, 100% { transform: translateX(0); }
            10%, 30%, 50%, 70%, 90% { transform: translateX(-3px); }
            20%, 40%, 60%, 80% { transform: translateX(3px); }
        }
        
        .shake {
            animation: shake 0.5s ease-in-out;
        }
        
        /* Product image zoom */
        .product-detail-image {
            overflow: hidden;
        }
        
        .product-detail-image img {
            transition: transform 0.5s ease;
        }
    `;
    document.head.appendChild(style);
});

/**
 * Manages the loading screen behavior
 */
function manageLoadingScreen() {
    const loadingScreen = document.getElementById('loading-screen');
    if (!loadingScreen) return;
    
    // Check for specific URLs where we want to always show the loading screen
    const currentUrl = window.location.href.toLowerCase();
    const isAuthPage = 
        currentUrl.includes('/account/login') || 
        currentUrl.includes('/account/register') || 
        currentUrl.includes('/account/logout');
    
    // Check if this is the first visit in this session
    const isFirstVisit = !sessionStorage.getItem('visited');
    
    if (isFirstVisit || isAuthPage) {
        // Show the loading screen
        if (isFirstVisit) {
            sessionStorage.setItem('visited', 'true');
        }
        
        // Hide after a short delay
        setTimeout(function() {
            loadingScreen.classList.add('loading-hide');
        }, 1500);
    } else {
        // Not first visit or auth page - immediately hide loading screen
        loadingScreen.classList.add('loading-hide');
    }
}

/**
 * Initializes interactive star rating system
 */
function initStarRatings() {
    const starContainers = document.querySelectorAll('.star-rating');
    
    starContainers.forEach(container => {
        const stars = container.querySelectorAll('i');
        let selectedRating = 0;
        let currentRating = 0;
        
        // Check if there's a value input associated with this rating
        const ratingInput = container.querySelector('input[type="hidden"]');
        if (ratingInput && ratingInput.value) {
            selectedRating = parseInt(ratingInput.value);
            currentRating = selectedRating;
            
            // Apply initial visual state
            stars.forEach((star, i) => {
                if (i < selectedRating) {
                    star.classList.add('filled');
                } else {
                    star.classList.remove('filled');
                }
            });
        } else {
            // If no input, count filled stars
            stars.forEach((star, i) => {
                if (star.classList.contains('filled')) {
                    currentRating = i + 1;
                    selectedRating = currentRating;
                }
            });
        }
        
        // Handle hover effect
        stars.forEach((star, index) => {
            star.addEventListener('mouseenter', () => {
                // Fill this star and all previous stars
                stars.forEach((s, i) => {
                    if (i <= index) {
                        s.classList.add('hover');
                    } else {
                        s.classList.remove('hover');
                    }
                });
            });
            
            // Handle click to set rating
            star.addEventListener('click', () => {
                selectedRating = index + 1;
                
                // Update visual state
                stars.forEach((s, i) => {
                    if (i < selectedRating) {
                        s.classList.add('filled');
                    } else {
                        s.classList.remove('filled');
                    }
                });
                
                // Update hidden input if exists
                if (ratingInput) {
                    ratingInput.value = selectedRating;
                    
                    // Trigger change event for any listeners
                    ratingInput.dispatchEvent(new Event('change'));
                }
                
                // If we're in a form, find and trigger submit button if exists
                const form = container.closest('form[data-auto-submit="true"]');
                if (form) {
                    const submitBtn = form.querySelector('button[type="submit"]');
                    if (submitBtn) {
                        submitBtn.click();
                    } else {
                        form.submit();
                    }
                }
            });
        });
        
        container.addEventListener('mouseleave', () => {
            stars.forEach(s => s.classList.remove('hover'));
        });
    });
}
