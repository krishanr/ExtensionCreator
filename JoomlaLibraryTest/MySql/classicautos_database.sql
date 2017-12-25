DROP TABLE IF EXISTS `customers`;

CREATE TABLE `customers` (
  `customerNumber` int(11) NOT NULL PRIMARY KEY,
  `customerName` varchar(50) NOT NULL KEY,
  `contactLastName` varchar(50) NOT NULL,
  `contactFirstName` varchar(50) NOT NULL,
  `phone` varchar(50) NOT NULL UNIQUE KEY,
  `addressLine1` varchar(50) NOT NULL,
  `addressLine2` varchar(50) NOT NULL,
  `city` varchar(50) NOT NULL,
  `state` ENUM('new', 'served', 'in progress', 'cancelled') DEFAULT 'new',
  `orders` SET('a', 'b', 'c', 'd'),
  `postalCode` varchar(15) DEFAULT NULL COMMENT '5 digit US postal code or 6 character CA postal code.',
  `country` varchar(50) NOT NULL DEFAULT 'United States',
  `salesRepEmployeeNumber` int(11) DEFAULT NULL,
  `creditLimit` decimal(10,2) DEFAULT NULL,
  KEY `salesRepEmployeeNumber` (`salesRepEmployeeNumber`),
  CONSTRAINT `customers_ibfk_1` FOREIGN KEY (`salesRepEmployeeNumber`) REFERENCES `employees` (`employeeNumber`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;