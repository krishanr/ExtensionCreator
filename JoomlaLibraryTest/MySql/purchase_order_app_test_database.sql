-- -----------------------------------------------------
-- Table `mydb`.`po_vendors`
-- -----------------------------------------------------
CREATE TABLE `#__po_vendors` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `published` TINYINT(1) NOT NULL DEFAULT 0,
  `first_name` VARCHAR(128) NULL,
  `last_name` VARCHAR(128) NULL,
  `company` VARCHAR(255) NULL,
  `terms` VARCHAR(16) NULL,
  `telephone` VARCHAR(16) NULL,
  `email` VARCHAR(100) NULL,
  `alt_email` VARCHAR(100) NULL,
  `address1` TEXT NULL,
  `address2` TEXT NULL,
  `city` VARCHAR(100) NULL,
  `state2` VARCHAR(100) NULL,
  `country` VARCHAR(100) NULL,
  `postal_code` VARCHAR(100) NULL,
  `account_num` VARCHAR(100) NULL,
  `notes` TEXT NULL,
  `created` DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
  `created_by` INT UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) ,
  INDEX `idx_company` (`company` ASC) ,
  INDEX `idx_email` (`email` ASC) ,
  INDEX `idx_status` (`published` ASC) ,
  INDEX `idx_account_num` (`account_num` ASC) ,
  INDEX `idx_address` (`address1`(256) ASC)   
) DEFAULT CHARSET=utf8;


-- -----------------------------------------------------
-- Table `mydb`.`po_divisions`
-- -----------------------------------------------------
CREATE TABLE `#__po_divisions` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `published` TINYINT(1) NOT NULL DEFAULT 0,
  `company_name` VARCHAR(128) NOT NULL DEFAULT '',
  `division_num` INT UNSIGNED NOT NULL,
  `division_name` VARCHAR(256) NULL,
  `address` VARCHAR(128) NULL,
  `other_address` VARCHAR(128) NULL,
  `country` VARCHAR(128) NULL,
  `city` VARCHAR(128) NULL,
  `state` VARCHAR(128) NULL,
  `postal_code` VARCHAR(128) NULL,
  `email` VARCHAR(128) NULL,
  `phone` VARCHAR(16) NULL,
  `fax` VARCHAR(16) NULL,
  `first_name` VARCHAR(128) NULL,
  `last_name` VARCHAR(128) NULL,
  `created` DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
  `created_by` INT UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) ,
  UNIQUE INDEX `uni_division_num` (`division_num` ASC) ,
  INDEX `idx_division_name` (`division_name` ASC) ,
  INDEX `idx_country` (`country` ASC)   
) DEFAULT CHARSET=utf8;


-- -----------------------------------------------------
-- Table `mydb`.`po_projects`
-- -----------------------------------------------------
CREATE TABLE `#__po_projects` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `published` TINYINT(1) NOT NULL DEFAULT 0,
  `division` INT UNSIGNED NOT NULL,
  `project_num` INT UNSIGNED NOT NULL,
  `description` VARCHAR(256) NULL,
  `budget` DECIMAL(11,2) NOT NULL DEFAULT 0.00,
  `billable` TINYINT(1) NOT NULL DEFAULT 0,
  `created` DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
  `created_by` INT UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) ,
  INDEX `fk_projects_divisions_idx` (`division` ASC) ,
  UNIQUE INDEX `uni_project_num` (`project_num` ASC) ,
  INDEX `idx_published` (`published` ASC) ,
  INDEX `idx_billable` (`billable` ASC) ,
  CONSTRAINT `fk_projects_divisions`
    FOREIGN KEY (`division`)
    REFERENCES `#__po_divisions` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION  
) DEFAULT CHARSET=utf8;


-- -----------------------------------------------------
-- Table `mydb`.`po_departments`
-- -----------------------------------------------------
CREATE TABLE `#__po_departments` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `published` TINYINT(1) NOT NULL DEFAULT 0,
  `division_num` INT UNSIGNED NOT NULL,
  `department_name` VARCHAR(256) NULL,
  `description` VARCHAR(256) NULL,
  `created` DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
  `created_by` INT UNSIGNED NULL DEFAULT 0,
  PRIMARY KEY (`id`) ,
  INDEX `fk_departments_divisions1_idx` (`division_num` ASC) ,
  CONSTRAINT `fk_departments_divisions`
    FOREIGN KEY (`division_num`)
    REFERENCES `#__po_divisions` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION  
) DEFAULT CHARSET=utf8;


-- -----------------------------------------------------
-- Table `mydb`.`po_purchases`
-- -----------------------------------------------------
CREATE TABLE `#__po_purchases` (
  `purchase_num` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `created` DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
  `created_by` INT UNSIGNED NOT NULL DEFAULT 0,
  `due_date` DATE NOT NULL DEFAULT '0000-00-00',
  `status` TINYINT(2) NULL,
  `terms` VARCHAR(16) NULL,
  `items_rec` TINYINT(1) NOT NULL DEFAULT 0,
  `sent_approv` TINYINT(1) NOT NULL DEFAULT 0,
  `sent_vendor` TINYINT(1) NOT NULL DEFAULT 0,
  `vendors_id` INT UNSIGNED NOT NULL,
  `division_num` INT UNSIGNED NOT NULL,
  `department_id` INT UNSIGNED NOT NULL,
  `project_id` INT UNSIGNED NOT NULL,
  `sales_tax` DECIMAL(4,2) NOT NULL DEFAULT 0.00,
  `shipping_amount` DECIMAL(11,2) NOT NULL DEFAULT 0.00,
  PRIMARY KEY (`purchase_num`) ,
  INDEX `fk_purchases_projects1_idx` (`project_id` ASC) ,
  INDEX `fk_purchases_departments1_idx` (`department_id` ASC) ,
  INDEX `fk_purchases_divisions1_idx` (`division_num` ASC) ,
  INDEX `fk_purchases_vendors1_idx` (`vendors_id` ASC) ,
  INDEX `idx_status` (`status` ASC) ,
  INDEX `idx_created` (`created` ASC) ,
  CONSTRAINT `fk_purchases_projects`
    FOREIGN KEY (`project_id`)
    REFERENCES `#__po_projects` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_purchases_departments`
    FOREIGN KEY (`department_id`)
    REFERENCES `#__po_departments` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_purchases_divisions`
    FOREIGN KEY (`division_num`)
    REFERENCES `#__po_divisions` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_purchases_vendors`
    FOREIGN KEY (`vendors_id`)
    REFERENCES `#__po_vendors` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION  
) DEFAULT CHARSET=utf8;


-- -----------------------------------------------------
-- Table `mydb`.`po_line_items`
-- -----------------------------------------------------
CREATE TABLE `#__po_line_items` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `published` TINYINT(1) NOT NULL DEFAULT 0,
  `item_num` VARCHAR(128) NOT NULL DEFAULT 0,
  `description` VARCHAR(128) NULL,
  `unit_type` TINYINT(2) NOT NULL,
  `amount` DECIMAL(11,2) NOT NULL DEFAULT 0.00,
  `discount` DECIMAL(4,2) NOT NULL DEFAULT 0.00,
  `taxable` TINYINT(1) NOT NULL DEFAULT 0,
  `created` DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
  `created_by` INT UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) ,
  UNIQUE INDEX `uni_item_num` (`item_num` ASC) ,
  INDEX `idx_description` (`description` ASC) ,
  INDEX `idx_unit_type` (`unit_type` ASC) ,
  INDEX `idx_published` (`published` ASC)   
) DEFAULT CHARSET=utf8;


-- -----------------------------------------------------
-- Table `mydb`.`po_activities`
-- -----------------------------------------------------
CREATE TABLE `#__po_activities` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `group` INT UNSIGNED NOT NULL,
  `heading` VARCHAR(128) NULL,
  PRIMARY KEY (`id`) ,
  INDEX `idx_group` (`group` ASC)   
) DEFAULT CHARSET=utf8;


-- -----------------------------------------------------
-- Table `mydb`.`po_purchase_items`
-- -----------------------------------------------------
CREATE TABLE `#__po_purchase_items` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `purchase_num` INT UNSIGNED NOT NULL,
  `ordering` INT UNSIGNED NOT NULL DEFAULT 0 COMMENT 'The order of this purchase item relative to the others for the given purchase order.',
  `item_num` VARCHAR(128) NOT NULL,
  `description` VARCHAR(128) NULL,
  `activity` INT UNSIGNED NULL,
  `unit_type` VARCHAR(32) NOT NULL,
  `quantity` DECIMAL(8) NOT NULL DEFAULT 0,
  `amount` DECIMAL(11,2) NOT NULL DEFAULT 0.00,
  `discount` DECIMAL(4,2) NOT NULL DEFAULT 0.00,
  `taxable` TINYINT(1) NOT NULL DEFAULT 0,
  `received` TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) ,
  INDEX `fk_purchase_items_activities1_idx` (`activity` ASC) ,
  INDEX `fk_purchase_items_purchases1_idx` (`purchase_num` ASC) ,
  CONSTRAINT `fk_purchase_items_activities1`
    FOREIGN KEY (`activity`)
    REFERENCES `#__po_activities` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_purchase_items_purchases1`
    FOREIGN KEY (`purchase_num`)
    REFERENCES `#__po_purchases` (`purchase_num`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION  
) DEFAULT CHARSET=utf8;


-- -----------------------------------------------------
-- Table `mydb`.`po_purchase_comments`
-- -----------------------------------------------------
CREATE TABLE `#__po_purchase_comments` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `purchase_num` INT UNSIGNED NOT NULL,
  `created` DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
  `created_by` INT UNSIGNED NOT NULL DEFAULT 0,
  `comment` TEXT NOT NULL DEFAULT '',
  PRIMARY KEY (`id`) ,
  INDEX `fk_purchase_comments_purchases1_idx` (`purchase_num` ASC) ,
  INDEX `idx_created_by` (`created_by` ASC) ,
  CONSTRAINT `fk_purchase_comments_purchases1`
    FOREIGN KEY (`purchase_num`)
    REFERENCES `#__po_purchases` (`purchase_num`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION  
) DEFAULT CHARSET=utf8;