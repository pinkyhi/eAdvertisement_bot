-- MySQL dump 10.13  Distrib 8.0.16, for Win64 (x86_64)
--
-- Host: localhost    Database: eadvertisement_bot
-- ------------------------------------------------------
-- Server version	8.0.16

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `advertisement`
--

DROP TABLE IF EXISTS `advertisement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `advertisement` (
  `channel_id` int(11) NOT NULL,
  `date_time` datetime NOT NULL,
  `top` int(11) NOT NULL DEFAULT '1',
  `alive` int(11) NOT NULL DEFAULT '24',
  `publication_snapshot` longtext NOT NULL,
  `user_id` int(11) NOT NULL,
  `advertisement_status_id` int(11) NOT NULL DEFAULT '1',
  `price` int(11) NOT NULL,
  PRIMARY KEY (`channel_id`,`date_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `advertisement`
--

LOCK TABLES `advertisement` WRITE;
/*!40000 ALTER TABLE `advertisement` DISABLE KEYS */;
/*!40000 ALTER TABLE `advertisement` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `advertisement_status`
--

DROP TABLE IF EXISTS `advertisement_status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `advertisement_status` (
  `advertisement_status_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`advertisement_status_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `advertisement_status`
--

LOCK TABLES `advertisement_status` WRITE;
/*!40000 ALTER TABLE `advertisement_status` DISABLE KEYS */;
INSERT INTO `advertisement_status` VALUES (1,'Awaiting'),(2,'Accepted'),(3,'Declined'),(4,'Working'),(5,'Successful'),(6,'Interrupted');
/*!40000 ALTER TABLE `advertisement_status` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `autobuy`
--

DROP TABLE IF EXISTS `autobuy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `autobuy` (
  `autobuy_id` int(11) NOT NULL AUTO_INCREMENT,
  `publication_snapshot` longtext NOT NULL,
  `interval` int(11) NOT NULL,
  `min_price` int(11) NOT NULL DEFAULT '0',
  `max_price` int(11) NOT NULL,
  `max_cpm` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  PRIMARY KEY (`autobuy_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `autobuy`
--

LOCK TABLES `autobuy` WRITE;
/*!40000 ALTER TABLE `autobuy` DISABLE KEYS */;
/*!40000 ALTER TABLE `autobuy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `autobuy_channel`
--

DROP TABLE IF EXISTS `autobuy_channel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `autobuy_channel` (
  `autobuy_id` int(11) NOT NULL,
  `channel_id` int(11) NOT NULL,
  PRIMARY KEY (`autobuy_id`,`channel_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `autobuy_channel`
--

LOCK TABLES `autobuy_channel` WRITE;
/*!40000 ALTER TABLE `autobuy_channel` DISABLE KEYS */;
/*!40000 ALTER TABLE `autobuy_channel` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `button`
--

DROP TABLE IF EXISTS `button`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `button` (
  `button_id` int(11) NOT NULL AUTO_INCREMENT,
  `publication_id` int(11) DEFAULT NULL,
  `string` varchar(512) DEFAULT NULL,
  PRIMARY KEY (`button_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `button`
--

LOCK TABLES `button` WRITE;
/*!40000 ALTER TABLE `button` DISABLE KEYS */;
/*!40000 ALTER TABLE `button` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `category`
--

DROP TABLE IF EXISTS `category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `category` (
  `category_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`category_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `category`
--

LOCK TABLES `category` WRITE;
/*!40000 ALTER TABLE `category` DISABLE KEYS */;
/*!40000 ALTER TABLE `category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `channel`
--

DROP TABLE IF EXISTS `channel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `channel` (
  `channel_id` int(11) NOT NULL,
  `link` varchar(256) DEFAULT NULL,
  `subscribers` int(11) NOT NULL DEFAULT '0',
  `coverage` int(11) NOT NULL DEFAULT '0',
  `cpm` int(11) DEFAULT NULL,
  `name` varchar(64) DEFAULT NULL,
  `description` varchar(1024) DEFAULT NULL,
  `ban` tinyint(1) NOT NULL DEFAULT '0',
  `user_id` int(11) NOT NULL,
  `date` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`channel_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `channel`
--

LOCK TABLES `channel` WRITE;
/*!40000 ALTER TABLE `channel` DISABLE KEYS */;
/*!40000 ALTER TABLE `channel` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `channel_category`
--

DROP TABLE IF EXISTS `channel_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `channel_category` (
  `channel_id` int(11) NOT NULL,
  `category_id` int(11) NOT NULL,
  PRIMARY KEY (`channel_id`,`category_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `channel_category`
--

LOCK TABLES `channel_category` WRITE;
/*!40000 ALTER TABLE `channel_category` DISABLE KEYS */;
/*!40000 ALTER TABLE `channel_category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `input`
--

DROP TABLE IF EXISTS `input`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `input` (
  `input_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `sum` double NOT NULL,
  `inputed` int(11) NOT NULL,
  `description` varchar(64) NOT NULL,
  PRIMARY KEY (`input_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `input`
--

LOCK TABLES `input` WRITE;
/*!40000 ALTER TABLE `input` DISABLE KEYS */;
/*!40000 ALTER TABLE `input` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `media`
--

DROP TABLE IF EXISTS `media`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `media` (
  `media_id` int(11) NOT NULL AUTO_INCREMENT,
  `publication_id` int(11) DEFAULT NULL,
  `path` varchar(512) DEFAULT NULL,
  PRIMARY KEY (`media_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `media`
--

LOCK TABLES `media` WRITE;
/*!40000 ALTER TABLE `media` DISABLE KEYS */;
/*!40000 ALTER TABLE `media` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `place`
--

DROP TABLE IF EXISTS `place`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `place` (
  `time` time NOT NULL,
  `channel_id` int(11) NOT NULL,
  PRIMARY KEY (`time`,`channel_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `place`
--

LOCK TABLES `place` WRITE;
/*!40000 ALTER TABLE `place` DISABLE KEYS */;
/*!40000 ALTER TABLE `place` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `publication`
--

DROP TABLE IF EXISTS `publication`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `publication` (
  `publication_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL,
  `text` varchar(2048) DEFAULT NULL,
  PRIMARY KEY (`publication_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `publication`
--

LOCK TABLES `publication` WRITE;
/*!40000 ALTER TABLE `publication` DISABLE KEYS */;
/*!40000 ALTER TABLE `publication` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `user` (
  `user_id` int(11) NOT NULL,
  `phone` varchar(24) DEFAULT NULL,
  `nickname` varchar(64) DEFAULT NULL,
  `balance` int(11) NOT NULL DEFAULT '0',
  `publications_limit` int(11) NOT NULL DEFAULT '5',
  `autobuys_limit` int(11) NOT NULL DEFAULT '5',
  `ban` tinyint(1) NOT NULL DEFAULT '0',
  `firstname` varchar(45) DEFAULT NULL,
  `lastname` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (458816638,NULL,'pinky_hi',0,0,0,0,'Иван','Владимиров'),(933004747,NULL,'memniyboh',0,0,0,0,'Мемный Бох',NULL);
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_channel`
--

DROP TABLE IF EXISTS `user_channel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `user_channel` (
  `user_id` int(11) NOT NULL,
  `channel_id` int(11) NOT NULL,
  `date` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`,`channel_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_channel`
--

LOCK TABLES `user_channel` WRITE;
/*!40000 ALTER TABLE `user_channel` DISABLE KEYS */;
/*!40000 ALTER TABLE `user_channel` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-11-02 14:48:31
