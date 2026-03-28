// Shopping Cart Manager
class ShoppingCart {
    constructor() {
        this.storageKey = 'gymShoppingCart';
        this.cart = this.loadCart();
    }

    loadCart() {
        const saved = localStorage.getItem(this.storageKey);
        return saved ? JSON.parse(saved) : [];
    }

    saveCart() {
        localStorage.setItem(this.storageKey, JSON.stringify(this.cart));
        this.updateCartUI();
    }

    addProduct(productId, productName, price, imageUrl, quantity = 1) {
        const existingItem = this.cart.find(item => item.productId === productId);
        
        if (existingItem) {
            existingItem.quantity += quantity;
        } else {
            this.cart.push({
                productId: productId,
                productName: productName,
                price: price,
                imageUrl: imageUrl,
                quantity: quantity
            });
        }
        
        this.saveCart();
        this.showNotification(`${productName} đã được thêm vào giỏ hàng!`, 'success');
    }

    removeProduct(productId) {
        this.cart = this.cart.filter(item => item.productId !== productId);
        this.saveCart();
    }

    updateQuantity(productId, quantity) {
        const item = this.cart.find(item => item.productId === productId);
        if (item) {
            item.quantity = Math.max(1, quantity);
            this.saveCart();
        }
    }

    getTotal() {
        return this.cart.reduce((total, item) => total + (item.price * item.quantity), 0);
    }

    getTotalItems() {
        return this.cart.reduce((total, item) => total + item.quantity, 0);
    }

    clearCart() {
        this.cart = [];
        this.saveCart();
    }

    checkout(deliveryAddress, paymentMethod, notes = '') {
        if (this.cart.length === 0) {
            alert('Giỏ hàng trống!');
            return false;
        }

        const checkoutData = {
            cartItems: this.cart,
            deliveryAddress: deliveryAddress,
            paymentMethod: paymentMethod,
            notes: notes
        };

        return fetch('/Payment/CheckoutProducts', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(checkoutData)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                cart.clearCart();
                window.location.href = `/Order/Success/${data.orderId}`;
                return true;
            } else {
                alert('Lỗi: ' + (data.message || 'Không thể xử lý đơn hàng'));
                return false;
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Lỗi kết nối');
            return false;
        });
    }

    updateCartUI() {
        const cartCount = document.getElementById('cart-count');
        const cartBadge = document.getElementById('cart-badge');
        const totalItems = this.getTotalItems();

        if (cartCount) {
            cartCount.textContent = totalItems;
            cartBadge.style.display = totalItems > 0 ? 'flex' : 'none';
        }
    }

    showNotification(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `cart-notification ${type}`;
        notification.textContent = message;
        notification.style.cssText = `
            position: fixed;
            bottom: 20px;
            right: 20px;
            padding: 15px 20px;
            background-color: ${type === 'success' ? '#28a745' : '#f36100'};
            color: white;
            border-radius: 5px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
            z-index: 9999;
            animation: slideIn 0.3s ease-in-out;
            font-weight: 600;
        `;
        
        document.body.appendChild(notification);
        
        setTimeout(() => {
            notification.style.animation = 'slideOut 0.3s ease-in-out';
            setTimeout(() => notification.remove(), 300);
        }, 3000);
    }
}

// Initialize cart
const cart = new ShoppingCart();

// Add styles for animations
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }

    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(400px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);

// Update cart UI on page load
document.addEventListener('DOMContentLoaded', () => {
    cart.updateCartUI();
});
