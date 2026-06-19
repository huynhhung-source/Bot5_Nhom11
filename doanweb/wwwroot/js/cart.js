// Shopping Cart Manager
class ShoppingCart {
    constructor() {
        this.storageKey = 'gymShoppingCart';
        this.cart = [];
        this._syncing = false;
    }

    isLoggedIn() {
        return document.body?.dataset?.loggedIn === 'true';
    }

    readRawStorage() {
        try {
            const saved = localStorage.getItem(this.storageKey);
            if (!saved) return [];
            let items = JSON.parse(saved);
            if (!Array.isArray(items)) {
                items = items && typeof items === 'object' ? Object.values(items) : [];
            }
            return items;
        } catch {
            return [];
        }
    }

    normalizeProductId(item) {
        const raw = item?.productId ?? item?.ProductId ?? item?.id ?? item?.Id ?? 0;
        const n = parseInt(raw, 10);
        return Number.isNaN(n) ? 0 : n;
    }

    normalizeItem(item) {
        return {
            productId: this.normalizeProductId(item),
            productName: item.productName || item.ProductName || '',
            price: Number(item.price ?? item.Price) || 0,
            imageUrl: item.imageUrl || item.ImageUrl || '',
            quantity: Math.max(0, parseInt(item.quantity ?? item.Quantity, 10) || 0),
            stockQuantity: item.stockQuantity != null
                ? parseInt(item.stockQuantity ?? item.StockQuantity, 10)
                : null,
            outOfStock: !!(item.outOfStock ?? item.OutOfStock),
            unavailable: !!(item.unavailable ?? item.Unavailable)
        };
    }

    setCart(items) {
        this.cart = (items || [])
            .map(item => this.normalizeItem(item))
            .filter(item => item.productId > 0 && item.quantity > 0);
        localStorage.setItem(this.storageKey, JSON.stringify(this.cart));
        this.updateCartUI();
    }

    async fetchCartFromServer() {
        if (!this.isLoggedIn()) {
            return false;
        }
        try {
            const response = await fetch('/Cart/Items');
            if (!response.ok) {
                return false;
            }
            const data = await response.json();
            if (Array.isArray(data)) {
                this.setCart(data);
                return true;
            }
        } catch (e) {
            console.error('fetchCartFromServer:', e);
        }
        return false;
    }

    async syncLocalToServer() {
        if (!this.isLoggedIn()) {
            return false;
        }
        const local = this.readRawStorage()
            .map(item => this.normalizeItem(item))
            .filter(item => item.productId > 0 && item.quantity > 0);

        if (local.length === 0) {
            return false;
        }

        try {
            const response = await fetch('/Cart/Sync', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(local)
            });
            const data = await response.json();
            if (data.success && Array.isArray(data.cart)) {
                this.setCart(data.cart);
                return true;
            }
        } catch (e) {
            console.error('syncLocalToServer:', e);
        }
        return false;
    }

    getMaxStock(item) {
        const stock = item?.stockQuantity;
        return typeof stock === 'number' && !Number.isNaN(stock) ? stock : null;
    }

    isOutOfStock(item) {
        if (!item) return true;
        if (item.unavailable) return true;
        if (item.outOfStock) return true;
        const max = this.getMaxStock(item);
        return max !== null && max <= 0;
    }

    isPurchasable(item) {
        if (!item || this.isOutOfStock(item)) return false;
        const max = this.getMaxStock(item);
        if (max === null) return item.quantity > 0;
        return item.quantity > 0 && item.quantity <= max;
    }

    getAvailableItems() {
        return this.cart.filter(item => this.isPurchasable(item));
    }

    getDisplayItemCount() {
        return this.cart.reduce((total, item) => total + (item.quantity || 0), 0);
    }

    async addProduct(productId, productName, price, imageUrl, quantity = 1, stockQuantity = null) {
        const pid = this.normalizeProductId({ productId });
        const qty = Math.max(1, parseInt(quantity, 10) || 1);

        if (!this.isLoggedIn()) {
            alert('Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng');
            return false;
        }

        try {
            const response = await fetch('/Cart/Add', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ productId: pid, quantity: qty })
            });

            const data = await response.json();

            if (!data.success) {
                this.showNotification(data.message || 'Không thể thêm vào giỏ hàng', 'error');
                return false;
            }

            if (Array.isArray(data.cart)) {
                this.setCart(data.cart);
            }

            this.showNotification(data.message || `${productName} đã được thêm vào giỏ hàng!`, 'success');
            return true;
        } catch (e) {
            console.error('addProduct:', e);
            this.showNotification('Lỗi kết nối khi thêm vào giỏ hàng', 'error');
            return false;
        }
    }

    async removeProduct(productId) {
        const pid = this.normalizeProductId({ productId });

        if (this.isLoggedIn()) {
            try {
                const response = await fetch('/Cart/Remove', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ productId: pid, quantity: 0 })
                });
                const data = await response.json();
                if (data.success && Array.isArray(data.cart)) {
                    this.setCart(data.cart);
                    return;
                }
            } catch (e) {
                console.error('removeProduct:', e);
            }
        }

        this.cart = this.cart.filter(item => item.productId !== pid);
        this.setCart(this.cart);
    }

    async updateQuantity(productId, quantity) {
        const pid = this.normalizeProductId({ productId });
        const qty = parseInt(quantity, 10) || 1;

        if (this.isLoggedIn()) {
            try {
                const response = await fetch('/Cart/UpdateQuantity', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ productId: pid, quantity: qty })
                });
                const data = await response.json();
                if (!data.success) {
                    this.showNotification(data.message || 'Không thể cập nhật số lượng', 'warning');
                    return false;
                }
                if (Array.isArray(data.cart)) {
                    this.setCart(data.cart);
                }
                return true;
            } catch (e) {
                console.error('updateQuantity:', e);
            }
        }

        const item = this.cart.find(i => i.productId === pid);
        if (item) {
            item.quantity = qty;
            this.setCart(this.cart);
        }
        return true;
    }

    getTotal() {
        return this.getAvailableItems().reduce((total, item) => total + (item.price * item.quantity), 0);
    }

    getTotalItems() {
        return this.getAvailableItems().reduce((total, item) => total + item.quantity, 0);
    }

    async clearCart() {
        this.cart = [];
        localStorage.removeItem(this.storageKey);
        if (this.isLoggedIn()) {
            try {
                await fetch('/Cart/Sync', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: '[]'
                });
            } catch (e) {
                console.error('clearCart:', e);
            }
        }
        this.updateCartUI();
    }

    checkout(deliveryAddress, paymentMethod, notes = '', transactionId = '') {
        const available = this.getAvailableItems();
        if (available.length === 0) {
            alert('Không có sản phẩm khả dụng để thanh toán.');
            return Promise.resolve(false);
        }

        return fetch('/Payment/CheckoutProducts', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                cartItems: available,
                deliveryAddress: deliveryAddress,
                paymentMethod: paymentMethod,
                transactionId: transactionId || null,
                notes: notes
            })
        })
        .then(async response => {
            const data = await response.json().catch(() => null);
            if (!response.ok || !data) {
                throw new Error(data?.message || 'Không thể xử lý đơn hàng');
            }
            return data;
        })
        .then(data => {
            if (data.success) {
                cart.clearCart();
                window.location.href = `/Order/Success/${data.orderId}`;
                return true;
            }
            alert('Lỗi: ' + (data.message || 'Không thể xử lý đơn hàng'));
            return false;
        })
        .catch(error => {
            console.error('checkout:', error);
            alert(error.message || 'Lỗi kết nối');
            return false;
        });
    }

    updateCartUI() {
        const cartCount = document.getElementById('cart-count');
        const cartBadge = document.getElementById('cart-badge');
        const count = this.getDisplayItemCount();

        if (cartCount) {
            cartCount.textContent = count;
            if (cartBadge) {
                cartBadge.style.display = count > 0 ? 'flex' : 'none';
            }
        }
    }

    showNotification(message, type = 'info') {
        const colors = {
            success: '#28a745',
            error: '#dc3545',
            warning: '#f36100',
            info: '#f36100'
        };

        const notification = document.createElement('div');
        notification.textContent = message;
        notification.style.cssText = `
            position: fixed; bottom: 20px; right: 20px; padding: 15px 20px;
            background-color: ${colors[type] || colors.info}; color: white;
            border-radius: 5px; box-shadow: 0 4px 12px rgba(0,0,0,0.2);
            z-index: 9999; font-weight: 600; max-width: 320px;
        `;
        document.body.appendChild(notification);
        setTimeout(() => notification.remove(), 3000);
    }
}

const cart = new ShoppingCart();

document.addEventListener('DOMContentLoaded', async () => {
    if (document.getElementById('cart-items-list')) {
        return;
    }

    if (cart.isLoggedIn()) {
        const loaded = await cart.fetchCartFromServer();
        if (!loaded) {
            await cart.syncLocalToServer();
        }
    } else {
        cart.setCart(cart.readRawStorage().map(i => cart.normalizeItem(i)).filter(i => i.productId > 0));
    }
});
