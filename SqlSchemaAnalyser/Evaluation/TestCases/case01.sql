CREATE TABLE customers (
    id INT PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    email_address VARCHAR(150) NOT NULL,
    phone1 VARCHAR(20),
    phone2 VARCHAR(20),
    signup_date DATETIME
);

CREATE TABLE addresses (
    id INT PRIMARY KEY,
    cust_id INT NOT NULL,
    line1 VARCHAR(200),
    city VARCHAR(100),
    postcode VARCHAR(20),
    is_default INT
);

ALTER TABLE addresses ADD CONSTRAINT fk_addr_customer FOREIGN KEY (cust_id) REFERENCES customers(id);

CREATE TABLE products (
    id INT PRIMARY KEY,
    product_name VARCHAR(150) NOT NULL,
    category_name VARCHAR(100),
    unit_price DECIMAL(10,2),
    stock_qty INT
);

CREATE TABLE orders (
    id INT PRIMARY KEY,
    cust_id INT NOT NULL,
    ship_addr_id INT,
    order_status VARCHAR(20),
    order_date DATETIME
);

ALTER TABLE orders ADD CONSTRAINT fk_orders_customer FOREIGN KEY (cust_id) REFERENCES customers(id);
ALTER TABLE orders ADD CONSTRAINT fk_orders_address FOREIGN KEY (ship_addr_id) REFERENCES addresses(id);

CREATE TABLE order_items (
    id INT PRIMARY KEY,
    order_id INT NOT NULL,
    prod_id INT NOT NULL,
    product_name VARCHAR(150),
    unit_price DECIMAL(10,2),
    qty INT
);

ALTER TABLE order_items ADD CONSTRAINT fk_items_order FOREIGN KEY (order_id) REFERENCES orders(id);
ALTER TABLE order_items ADD CONSTRAINT fk_items_product FOREIGN KEY (prod_id) REFERENCES products(id);

CREATE INDEX idx_customers_signup ON customers(signup_date);
CREATE UNIQUE INDEX idx_products_name ON products(product_name);