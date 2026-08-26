CREATE TABLE authors (
    id INT PRIMARY KEY,
    author_name VARCHAR(100) NOT NULL,
    author_email VARCHAR(150) NOT NULL,
    bio TEXT
);

CREATE TABLE categories (
    id INT PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL
);

CREATE TABLE posts (
    id INT PRIMARY KEY,
    author_id INT NOT NULL,
    cat_id INT,
    tag1 VARCHAR(30),
    tag2 VARCHAR(30),
    tag3 VARCHAR(30),
    title VARCHAR(200),
    body TEXT,
    status VARCHAR(20),
    published_at DATETIME
);

ALTER TABLE posts ADD CONSTRAINT fk_posts_author FOREIGN KEY (author_id) REFERENCES authors(id);
ALTER TABLE posts ADD CONSTRAINT fk_posts_category FOREIGN KEY (cat_id) REFERENCES categories(id);

CREATE TABLE comments (
    id INT PRIMARY KEY,
    post_id INT NOT NULL,
    commenter_name VARCHAR(100),
    commenter_email VARCHAR(150),
    comment_text TEXT,
    posted_at DATETIME,
    approved INT
);

ALTER TABLE comments ADD CONSTRAINT fk_comments_post FOREIGN KEY (post_id) REFERENCES posts(id);

CREATE TABLE newsletter_subscribers (
    id INT PRIMARY KEY,
    sub_email VARCHAR(150) NOT NULL,
    fav_category_id INT,
    fav_category_name VARCHAR(100),
    subscribed_at DATETIME
);

ALTER TABLE newsletter_subscribers ADD CONSTRAINT fk_subs_category FOREIGN KEY (fav_category_id) REFERENCES categories(id);

CREATE INDEX idx_posts_published ON posts(published_at);
CREATE UNIQUE INDEX idx_authors_email ON authors(author_email);