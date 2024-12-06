/* NOTE: Run this after the initial Crunchbase import */

/* Change DATETIME to DATETIME(6), set default = now */
ALTER TABLE companyapi.organizations 
CHANGE COLUMN `CreatedAt` `CreatedAt` DATETIME(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ,
CHANGE COLUMN `UpdatedAt` `UpdatedAt` DATETIME(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ;

ALTER TABLE companyapi.locations 
CHANGE COLUMN `CreatedAt` `CreatedAt` DATETIME(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ,
CHANGE COLUMN `UpdatedAt` `UpdatedAt` DATETIME(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ;

ALTER TABLE companyapi.departments 
CHANGE COLUMN `CreatedAt` `CreatedAt` DATETIME(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ,
CHANGE COLUMN `UpdatedAt` `UpdatedAt` DATETIME(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ;

/* Add FULLTEXT Indexes */
CREATE FULLTEXT INDEX `IX_Organizations_OrganizationName` ON companyapi.organizations(`OrganizationName`);
CREATE FULLTEXT INDEX `IX_Locations_LocationName` ON companyapi.locations(`LocationName`);
CREATE FULLTEXT INDEX `IX_Departments_DepartmentName` ON companyapi.departments(`DepartmentName`);
