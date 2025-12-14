-- ============================================
-- MODERNIZATION SCHEMA ADDITIONS
-- ============================================

-- Use the coziest database
USE coziest;

-- ============================================
-- NOTIFICATIONS TABLE
-- ============================================
CREATE TABLE IF NOT EXISTS notifications (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    type INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    read_at TIMESTAMP NULL,
    is_read TINYINT DEFAULT 0,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    INDEX idx_user_unread (user_id, is_read),
    INDEX idx_created (created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- NOTIFICATION PREFERENCES TABLE
-- ============================================
CREATE TABLE IF NOT EXISTS notification_preferences (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    notification_type INT NOT NULL,
    email_enabled TINYINT DEFAULT 1,
    sms_enabled TINYINT DEFAULT 1,
    push_enabled TINYINT DEFAULT 1,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    UNIQUE KEY unique_user_type (user_id, notification_type)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- PRODUCT VIEWS TABLE (for analytics)
-- ============================================
CREATE TABLE IF NOT EXISTS product_views (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    product_id INT NOT NULL,
    view_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    duration_seconds INT DEFAULT 0,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL,
    FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
    INDEX idx_product_date (product_id, view_date),
    INDEX idx_user_date (user_id, view_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- LOYALTY ACCOUNTS TABLE
-- ============================================
CREATE TABLE IF NOT EXISTS loyalty_accounts (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL UNIQUE,
    points INT DEFAULT 0,
    tier INT DEFAULT 1, -- 1=Bronze, 2=Silver, 3=Gold, 4=Platinum
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    INDEX idx_tier (tier),
    INDEX idx_points (points)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- LOYALTY TRANSACTIONS TABLE
-- ============================================
CREATE TABLE IF NOT EXISTS loyalty_transactions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    points INT NOT NULL,
    type VARCHAR(50) NOT NULL, -- 'earned' or 'redeemed'
    order_id BIGINT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE SET NULL,
    INDEX idx_user_date (user_id, created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- LOYALTY REWARDS TABLE
-- ============================================
CREATE TABLE IF NOT EXISTS loyalty_rewards (
    id INT AUTO_INCREMENT PRIMARY KEY,
    reward_name VARCHAR(255) NOT NULL,
    points_required INT NOT NULL,
    discount_amount DECIMAL(10, 2),
    description TEXT,
    is_active TINYINT DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_active (is_active)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- SEARCH HISTORY TABLE
-- ============================================
CREATE TABLE IF NOT EXISTS search_history (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    search_term VARCHAR(255) NOT NULL,
    results_count INT DEFAULT 0,
    search_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL,
    INDEX idx_user_date (user_id, search_date),
    INDEX idx_search_term (search_term)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- PRODUCT RECOMMENDATIONS TABLE
-- ============================================
CREATE TABLE IF NOT EXISTS product_recommendations (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    recommended_product_id INT NOT NULL,
    score DECIMAL(5, 2), -- Recommendation score
    type VARCHAR(50), -- 'trending', 'similar', 'frequently_bought', 'personalized'
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (recommended_product_id) REFERENCES products(id) ON DELETE CASCADE,
    INDEX idx_user_score (user_id, score DESC),
    INDEX idx_created (created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- SAMPLE DATA
-- ============================================

-- Insert sample loyalty rewards
INSERT IGNORE INTO loyalty_rewards (reward_name, points_required, discount_amount, description, is_active) VALUES
('?100 Discount', 100, 100, 'Get ?100 off your next purchase', 1),
('?250 Discount', 250, 250, 'Get ?250 off your next purchase', 1),
('?500 Discount', 500, 500, 'Get ?500 off your next purchase', 1),
('Free Shipping', 150, 150, 'Free shipping on any order', 1),
('Birthday Bonus', 50, 50, '?50 discount on your birthday month', 1);

-- Insert sample notification preferences for existing users
INSERT IGNORE INTO notification_preferences (user_id, notification_type, email_enabled, sms_enabled, push_enabled)
SELECT u.id, 1, 1, 1, 1 FROM users u WHERE NOT EXISTS (
    SELECT 1 FROM notification_preferences WHERE user_id = u.id
);

-- ============================================
-- VIEWS FOR ANALYTICS
-- ============================================

-- Daily sales summary view
CREATE OR REPLACE VIEW daily_sales_summary AS
SELECT 
    DATE(o.order_date) as sale_date,
    COUNT(o.id) as order_count,
    SUM(o.total_amount) as daily_revenue,
    AVG(o.total_amount) as avg_order_value,
    COUNT(DISTINCT o.user_id) as unique_customers
FROM orders o
WHERE o.status != 'Cancelled'
GROUP BY DATE(o.order_date);

-- Product performance view
CREATE OR REPLACE VIEW product_performance AS
SELECT 
    p.id,
    p.product_name,
    p.category,
    p.price,
    COUNT(DISTINCT oi.order_id) as times_sold,
    SUM(oi.quantity) as total_units_sold,
    COUNT(pv.id) as view_count,
    ROUND(COUNT(DISTINCT oi.order_id) / COUNT(pv.id) * 100, 2) as conversion_rate,
    AVG(r.rating) as avg_rating,
    COUNT(r.id) as review_count
FROM products p
LEFT JOIN order_items oi ON p.id = oi.product_id
LEFT JOIN product_views pv ON p.id = pv.product_id
LEFT JOIN reviews r ON p.id = r.product_id
GROUP BY p.id;

-- Customer metrics view
CREATE OR REPLACE VIEW customer_metrics AS
SELECT 
    u.id as user_id,
    u.username,
    u.email,
    COUNT(DISTINCT o.id) as total_orders,
    SUM(o.total_amount) as lifetime_value,
    AVG(o.total_amount) as avg_order_value,
    MAX(o.order_date) as last_order_date,
    DATEDIFF(NOW(), MAX(o.order_date)) as days_since_last_order
FROM users u
LEFT JOIN orders o ON u.id = o.user_id AND o.status != 'Cancelled'
GROUP BY u.id;

-- ============================================
-- INDEXES FOR PERFORMANCE
-- ============================================

CREATE INDEX idx_notifications_user_read ON notifications(user_id, is_read);
CREATE INDEX idx_notifications_created ON notifications(created_at);
CREATE INDEX idx_product_views_date ON product_views(view_date);
CREATE INDEX idx_loyalty_accounts_tier ON loyalty_accounts(tier);
CREATE INDEX idx_loyalty_trans_user ON loyalty_transactions(user_id);
CREATE INDEX idx_search_history_user ON search_history(user_id);

-- ============================================
-- END OF MODERNIZATION SCHEMA
-- ============================================
