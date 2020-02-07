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
  `channel_id` bigint(20) NOT NULL,
  `date_time` datetime NOT NULL,
  `top` int(11) DEFAULT '1',
  `alive` int(11) DEFAULT '24',
  `publication_snapshot` longtext,
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
INSERT INTO `advertisement` VALUES (-1001388506925,'2020-02-06 20:20:00',1,24,'{\"Publication_Id\":45,\"Name\":\"newPost\",\"User_Id\":458816638,\"Text\":null,\"Media\":[{\"Media_Id\":16,\"Publication_Id\":45,\"Path\":\"AgACAgIAAxkBAAIKoF45jbN-s0lqdfsHw1WxtJ0bThndAALPrDEbaFPRScGIg02pjoG27FnLDgAEAQADAgADeQAD78YBAAEYBA\"}],\"Buttons\":[{\"Button_Id\":5,\"Publication_Id\":45,\"Text\":\"Button 1\",\"Url\":\"http://example1.com\"},{\"Button_Id\":6,\"Publication_Id\":45,\"Text\":\"Button 2\",\"Url\":\"http://example2.com\"}]}',458816638,2,2),(-1001388506925,'2020-02-09 21:40:00',0,0,NULL,0,9,0);
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
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `advertisement_status`
--

LOCK TABLES `advertisement_status` WRITE;
/*!40000 ALTER TABLE `advertisement_status` DISABLE KEYS */;
INSERT INTO `advertisement_status` VALUES (1,'Awaiting'),(2,'Accepted'),(3,'Declined'),(4,'Working'),(5,'Successful'),(6,'Interrupted'),(7,'New'),(8,'Init Confirmation'),(9,'Own');
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
  `channel_id` bigint(20) NOT NULL,
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
  `text` varchar(32) NOT NULL,
  `url` varchar(512) DEFAULT NULL,
  PRIMARY KEY (`button_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `button`
--

LOCK TABLES `button` WRITE;
/*!40000 ALTER TABLE `button` DISABLE KEYS */;
INSERT INTO `button` VALUES (3,31,'Button 1','http://example1.com'),(4,31,'Button 2','http://example2.com'),(5,45,'Button 1','http://example1.com'),(6,45,'Button 2','http://example2.com');
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
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `category`
--

LOCK TABLES `category` WRITE;
/*!40000 ALTER TABLE `category` DISABLE KEYS */;
INSERT INTO `category` VALUES (4,'Humor'),(5,'Comics'),(6,'Drugs'),(7,'Politics');
/*!40000 ALTER TABLE `category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `channel`
--

DROP TABLE IF EXISTS `channel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `channel` (
  `channel_id` bigint(20) NOT NULL,
  `link` varchar(256) DEFAULT NULL,
  `subscribers` int(11) NOT NULL DEFAULT '0',
  `coverage` int(11) NOT NULL DEFAULT '0',
  `cpm` int(11) DEFAULT NULL,
  `name` varchar(64) NOT NULL,
  `description` varchar(1024) DEFAULT NULL,
  `ban` tinyint(1) NOT NULL DEFAULT '0',
  `user_id` bigint(20) NOT NULL,
  `date` datetime DEFAULT CURRENT_TIMESTAMP,
  `price` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`channel_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `channel`
--

LOCK TABLES `channel` WRITE;
/*!40000 ALTER TABLE `channel` DISABLE KEYS */;
INSERT INTO `channel` VALUES (-1001490275304,'https://t.me/joinchat/AAAAAFjTy-g9D20fp_fY4w',2,0,NULL,'Vaniiidlo',NULL,0,517407871,'2019-11-13 05:57:21',0),(-1001488041253,'https://t.me/joinchat/AAAAAFixtSWUaYq_RFusGw',4,2,200,'TestPrivacy','suck that',0,933004747,'2019-11-12 18:34:01',190),(-1001471666826,'https://t.me/joinchat/AAAAAFe32or89lJ6nlOPng',2,0,NULL,'I love my kat☺️?','‘Super myau’',0,401093470,'2019-11-20 23:03:26',180),(-1001436122434,'https://t.me/joinchat/AAAAAFWZfUL4YxNNvPi65A',2,0,100,'Tttttf',NULL,0,933004747,'2019-11-18 10:10:06',170),(-1001388506925,'https://t.me/joinchat/AAAAAFLC7y3-jkMLw_0Gow',2,0,100,'Huyyyyy','Huynyaaa',0,458816638,'2020-01-27 18:52:29',2),(-1001335316790,'https://t.me/joinchat/AAAAAE-XUTZtVdxgEfBuAA',4,0,NULL,'тысяча хуев тебе в рот виталик',NULL,0,357927075,'2019-11-18 13:48:36',160),(-1001292420092,'https://t.me/joinchat/AAAAAE0Iw_z3cMCqvnwuJg',2,0,112,'Pizdiosss',NULL,0,458816638,'2020-01-27 18:55:19',0),(-1001281342955,'https://t.me/joinchat/AAAAAExfvevDvegLORYxcg',16088,2461,NULL,'тысяча хуёв тебе в рот, олег',NULL,0,811048411,'2019-11-13 14:14:08',120),(-1001210790589,'https://t.me/joinchat/AAAAAEgrMr184y3Rx7n7aA',2,0,627,'Anal','Hdjebrn',0,458816638,'2020-01-27 18:46:14',130),(-1001204934258,'https://t.me/joinchat/AAAAAEfR1nJatQ_W-3GNtw',3,3,NULL,'LastPrivacyChannelTes',NULL,0,933004747,'2019-12-12 16:15:13',140),(-1001152144911,'https://t.me/joinchat/AAAAAESsVg9786UpTOqYMg',2,0,NULL,'Hhsh',NULL,0,933004747,'2019-11-20 22:57:56',150);
/*!40000 ALTER TABLE `channel` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `channel_category`
--

DROP TABLE IF EXISTS `channel_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `channel_category` (
  `channel_id` bigint(20) NOT NULL,
  `category_id` int(11) NOT NULL,
  PRIMARY KEY (`channel_id`,`category_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `channel_category`
--

LOCK TABLES `channel_category` WRITE;
/*!40000 ALTER TABLE `channel_category` DISABLE KEYS */;
INSERT INTO `channel_category` VALUES (-1001471666826,6),(-1001281342955,4),(-1001281342955,5);
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
  `user_id` bigint(20) NOT NULL,
  `sum` double NOT NULL,
  `inputed` int(11) NOT NULL,
  `description` varchar(64) NOT NULL,
  PRIMARY KEY (`input_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `input`
--

LOCK TABLES `input` WRITE;
/*!40000 ALTER TABLE `input` DISABLE KEYS */;
INSERT INTO `input` VALUES (5,458816638,1,1,'1');
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
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `media`
--

LOCK TABLES `media` WRITE;
/*!40000 ALTER TABLE `media` DISABLE KEYS */;
INSERT INTO `media` VALUES (16,45,'AgACAgIAAxkBAAIKoF45jbN-s0lqdfsHw1WxtJ0bThndAALPrDEbaFPRScGIg02pjoG27FnLDgAEAQADAgADeQAD78YBAAEYBA');
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
  `channel_id` bigint(20) NOT NULL,
  `place_id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`time`,`channel_id`),
  UNIQUE KEY `place_id_UNIQUE` (`place_id`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `place`
--

LOCK TABLES `place` WRITE;
/*!40000 ALTER TABLE `place` DISABLE KEYS */;
INSERT INTO `place` VALUES ('20:20:00',-1001388506925,24);
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
  `user_id` bigint(20) DEFAULT NULL,
  `text` varchar(2048) DEFAULT NULL,
  PRIMARY KEY (`publication_id`)
) ENGINE=InnoDB AUTO_INCREMENT=46 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `publication`
--

LOCK TABLES `publication` WRITE;
/*!40000 ALTER TABLE `publication` DISABLE KEYS */;
INSERT INTO `publication` VALUES (30,'My first post',933004747,NULL),(31,'newPost',933004747,'Оо'),(32,'newPost',933004747,NULL),(36,'newPost',517407871,NULL),(37,'newPost',517407871,NULL),(45,'newPost',458816638,NULL);
/*!40000 ALTER TABLE `publication` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `user` (
  `user_id` bigint(20) NOT NULL,
  `phone` varchar(24) DEFAULT NULL,
  `nickname` varchar(64) DEFAULT NULL,
  `balance` int(11) NOT NULL DEFAULT '0',
  `ban` bit(1) NOT NULL,
  `firstname` varchar(64) DEFAULT NULL,
  `lastname` varchar(64) DEFAULT NULL,
  `language` varchar(32) DEFAULT NULL,
  `stopped` bit(1) NOT NULL,
  `user_state_id` bigint(20) NOT NULL,
  `object_id` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (357927075,NULL,'Sub2Ch',0,_binary '\0','SHIIIITT',NULL,NULL,_binary '\0',-1001335316790,0),(401093470,NULL,NULL,0,_binary '\0','Вероника','Балаклицкая','en',_binary '',0,0),(458816638,NULL,'pinky_hi',999996,_binary '\0','Иван','Владимиров','ru',_binary '',0,-1001388506925),(517407871,NULL,'zlobste',0,_binary '\0','Николай','Крайнюк','ru',_binary '\0',0,0),(811048411,NULL,'olejchanskiy',0,_binary '\0','олег','(не олег)',NULL,_binary '\0',0,-1001281342955),(933004747,NULL,'memniyboh',0,_binary '\0','Мемный Бох',NULL,'ru',_binary '\0',0,-1001204934258);
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_state`
--

DROP TABLE IF EXISTS `user_state`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `user_state` (
  `user_state_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `state` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`user_state_id`)
) ENGINE=InnoDB AUTO_INCREMENT=902 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_state`
--

LOCK TABLES `user_state` WRITE;
/*!40000 ALTER TABLE `user_state` DISABLE KEYS */;
INSERT INTO `user_state` VALUES (0,'Null'),(1,'Waiting for a new channel'),(2,'Channel menu'),(3,'Waiting for a channel to buy '),(101,'101*=add img to post with id *'),(102,'102*=add btn to post with id *'),(103,'103*=chng text in post with id *'),(104,'104*=cnhg name of post with id *'),(201,'201*=add place to chnl with id *'),(202,'202*=chng cpm to chnl with id *'),(203,'203*=chng desc to chnl with id *'),(901,'Waiting for a time for own sold');
/*!40000 ALTER TABLE `user_state` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-02-07 22:21:02
