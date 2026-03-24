-- Incremental migration for existing databases.
-- Safely renames log tables to the convention used by AuditableEntityAttribute.

SET FOREIGN_KEY_CHECKS=0;

RENAME TABLE
    ClientLogs TO client_log,
    ProductLogs TO product_log,
    OrderLogs TO order_log,
    OrderProductsLogs TO order_product_log;

SET FOREIGN_KEY_CHECKS=1;
